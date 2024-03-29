﻿using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Nexus.CompanyAPI.DTO;
using Nexus.CompanyAPI.Services;
using Nexus.SharedKernel.Contracts.Project;
using NSubstitute;
using Polly;
using Polly.Registry;

namespace Nexus.CompanyAPI.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class ProjectServiceTests
{
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    private readonly HttpMessageHandler _mockHandler;
    private readonly ILogger<ProjectService> _logger;
    private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;

    public ProjectServiceTests()
    {
        MapperConfiguration config = new (cfg => { cfg.CreateMap<ProjectResponseModel, ProjectSummaryDto>(); });

        _mapper = config.CreateMapper();
        _mockHandler = Substitute.For<HttpMessageHandler>();
        _client = new HttpClient(_mockHandler);
        _logger = Substitute.For<ILogger<ProjectService>>();
        _resiliencePipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();
    }

    [Fact]
    public async Task GetProjectsByCompanyIdAsync_ReturnsProjects_WhenStatusCodeIsSuccess()
    {
        // Arrange
        int companyId = 1;
        List<ProjectResponseModel> expectedProjects = new()
        {
            new ()
            {
                Id = 1,
                Name = "Project 1",
                CompanyId = 1,
            },
            new ()
            {
                Id = 2,
                Name = "Project 2",
                CompanyId = 1,
            },
        };

        Task<HttpResponseMessage> response = Task.FromResult<HttpResponseMessage>(new (HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedProjects),
        });

        MethodInfo? method = _mockHandler
            .GetType()
            .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        Assert.NotNull(method);
        
#pragma warning disable NS1000
#pragma warning disable NS1004
        method.Invoke(_mockHandler, new object?[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
            .Returns(response);
#pragma warning restore NS1004
#pragma warning restore NS1000
        
        _resiliencePipelineProvider.GetPipeline<List<ProjectSummaryDto>>(Arg.Any<string>())
            .Returns(ResiliencePipeline<List<ProjectSummaryDto>>.Empty);
        ProjectService service = new (_client, _mapper, _logger, _resiliencePipelineProvider);

        // Act
        List<ProjectSummaryDto> actualProjects = await service.GetProjectsByCompanyIdAsync(companyId);

        // Assert
        Assert.Equal(expectedProjects.Count, actualProjects.Count);
    }
    
    [Fact]
    public async Task GetProjectsByCompanyIdAsync_ReturnsEmptyList_WhenStatusCodeIsNotSuccess()
    {
        // Arrange
        int companyId = 1;
    
        _resiliencePipelineProvider.GetPipeline<List<ProjectSummaryDto>>(Arg.Any<string>())
            .Returns(ResiliencePipeline<List<ProjectSummaryDto>>.Empty);
        
        Task<HttpResponseMessage> response = Task.FromResult<HttpResponseMessage>(new (HttpStatusCode.BadRequest));
        
        MethodInfo? method = _mockHandler
            .GetType()
            .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        Assert.NotNull(method);
        
#pragma warning disable NS1000
#pragma warning disable NS1004
        method.Invoke(_mockHandler, new object?[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
            .Returns(response);
#pragma warning restore NS1004
#pragma warning restore NS1000
        
        ProjectService service = new (_client, _mapper, _logger, _resiliencePipelineProvider);
        
        // Act
        List<ProjectSummaryDto> actualProjects = await service.GetProjectsByCompanyIdAsync(companyId);
    
        // Assert
        Assert.Empty(actualProjects);
    }
}