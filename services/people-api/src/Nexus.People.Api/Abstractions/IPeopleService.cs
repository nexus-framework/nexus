using LanguageExt.Common;
using Nexus.PeopleAPI.DTO;
using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Abstractions;

public interface IPeopleService
{
    Task<List<PersonDto>> GetAllAsync();

    Task<Result<Person>> CreateAsync(Person personSummary);

    Task<Result<PersonDto>> GetByIdAsync(int id);

    Task<Result<Person>> UpdateNameAsync(int id, string name);
    
    Task DeleteAsync(int id);
}