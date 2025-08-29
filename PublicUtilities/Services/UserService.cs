using Microsoft.EntityFrameworkCore;
using PublicUtilities.Data;
using PublicUtilities.Dtos.Users;
using PublicUtilities.Models;

namespace PublicUtilities.Services;

public interface IUserService
{
    Task<CreateUserResultDto> CreateUser(CreateUserDto user);
    Task<DeleteUserResultDto> DeleteUser(DeleteUserDto user);
    Task<List<User>> GetAllUsers();
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreateUserResultDto> CreateUser(CreateUserDto user)
    {
        var checkUser = _context.Users.FirstOrDefault(x => x.PersonalAccount == user.PersonalAccount);
        if (checkUser is not null)
            return new CreateUserResultDto() { IsAdd = false, Message = $"Лицевой счет {user.PersonalAccount} существует" };

        _context.Users.Add(new User() { PersonalAccount = user.PersonalAccount });
        await _context.SaveChangesAsync();

        return new CreateUserResultDto() { IsAdd = true, Message = "Лицевой счет успешно добавлен" };
    }

    public async Task<DeleteUserResultDto> DeleteUser(DeleteUserDto user)
    {
        var delUser = await _context.Users.FirstOrDefaultAsync(x => x.PersonalAccount == user.PersonalAccount);
        if (delUser is null) return new DeleteUserResultDto() { IsDeleted = false, Message = $"Пользователь с лицевым счетом {user.PersonalAccount} не найден" };
        
        _context.Users.Remove(delUser);
        await _context.SaveChangesAsync();

        return new DeleteUserResultDto() { IsDeleted = true, Message = $"Пользователь с лицевым счетом {user.PersonalAccount} удален" };
    }

    public Task<List<User>> GetAllUsers()
    {
        return _context.Users
            .Include(u => u.СalculationResults)
            .Include(u => u.Metrics)
            .Include(u => u.ResidentPeriods)
            .ToListAsync();
    }
}
