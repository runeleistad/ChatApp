using ChatApp.Data.Models;
using ChatApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    [Authorize]
    public class MessageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(string fromId, long roomId, string content)
        {
            Console.WriteLine($"{fromId} - {roomId} - {content}");
            if (string.IsNullOrWhiteSpace(fromId)) 
                throw new ArgumentNullException(nameof(fromId));

            await _dbContext.Message.AddAsync(new Message { Content = content, RoomId = roomId, SenderId = fromId, SentTime = DateTime.Now });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Message>> GetMessagesFromRoom(long roomId)
        {
            ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
            if (user == null) 
                return [];

            string? userId = _userManager.GetUserId(user);
            RoomUser? roomUser = await _dbContext.RoomUser.Where(ru => ru.UserId == userId && ru.RoomId == roomId).SingleOrDefaultAsync();
            if (roomUser == default)
                return [];

            return await _dbContext.Message.Where(m => m.RoomId == roomId).Include(m => m.Sender).OrderByDescending(m => m.SentTime).ToListAsync();
        }

        public async Task<Room> GetRoomById(long roomId)
        {
            return await _dbContext.Room.Include(r => r.RoomUsers).Where(r => r.Id == roomId).SingleAsync() ?? new Room();
        }
    }
}
