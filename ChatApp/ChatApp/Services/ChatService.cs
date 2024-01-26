using ChatApp.Data;
using ChatApp.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    [Authorize]
    public class ChatService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _currentUserId => _userManager.GetUserId(_httpContextAccessor.HttpContext?.User!)!;
        public ChatService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        { 
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserId() { return _currentUserId; }

        public async Task<List<Contact>> GetAllContacts()
        {
            string userId = _currentUserId;
            return await _dbContext.Contact.Include(c => c.ContactUser).Where(c => c.OwnerUserId == userId).ToListAsync() ?? [];
        }

        public async Task<List<ApplicationUser>> SearchForNewContact(string searchPhrase)
        {
            if (string.IsNullOrWhiteSpace(searchPhrase))
                throw new ArgumentNullException(nameof(searchPhrase));

            List<ApplicationUser> result = await _dbContext.ApplicationUser
                .Where(user => user.UserName!.Contains(searchPhrase) || user.Email!.Contains(searchPhrase))
                .OrderBy(user => user.UserName)
                .Take(10)
                .ToListAsync();
            if (result.Count == 0)
                return [];

            // fjerne eksisterende kontakter
            List<Contact> contacts = await GetAllContacts();
            foreach (var contact in contacts)
            {
                ApplicationUser existingContact = result.Where(au => au.Id == contact.ContactUserId).Single();
                result.Remove(existingContact);
            }

            string? username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            result.RemoveAll(user => user.UserName == username);
            return result.Distinct().ToList();
        }

        public async Task AddContact(ApplicationUser newContact)
        {
            string currentUserId = _currentUserId;
            if (newContact.Id == currentUserId)
                throw new Exception("Cannot add yourself as a contact");

            var exists = await _dbContext.Contact.Where(c => c.OwnerUserId == currentUserId && c.ContactUserId == newContact.Id).ToListAsync();
            if (exists.Count > 0)
                throw new Exception("User already has this contact");

            // only create Room And RoomUsers if new contact hasn't already added current user
            if (await _dbContext.Contact.Where(c => c.OwnerUserId == newContact.Id && c.ContactUserId == currentUserId).SingleOrDefaultAsync() == default)
            {
                // Create new chat room between current user and new contact
                Room newRoom = new Room { Name = "ROOMName", Description = "Just the description" };
                await _dbContext.Room.AddAsync(newRoom);
                await _dbContext.SaveChangesAsync();

                // Create room users for both
                RoomUser contactRoomUser = new RoomUser { RoomId = newRoom.Id, UserId = newContact.Id };
                await _dbContext.RoomUser.AddAsync(contactRoomUser);
                RoomUser currentRoomUser = new RoomUser { RoomId = newRoom.Id, UserId = currentUserId };
                await _dbContext.RoomUser.AddAsync(currentRoomUser);
            }

            await _dbContext.Contact.AddAsync(new Contact { ContactUserId = newContact.Id, OwnerUserId = currentUserId });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<long> GetChatRoomIdByContact(string contactUserId)
        {
            if (string.IsNullOrEmpty(contactUserId))
                throw new ArgumentNullException(nameof(contactUserId));

            string currentUserId = _currentUserId;
            List<RoomUser> currentUserRooms = await _dbContext.RoomUser.Where(ru => ru.UserId == currentUserId).ToListAsync();
            List<long> roomIds = currentUserRooms.Select(cur => cur.RoomId).ToList();
            RoomUser? contactUserRooms = await _dbContext.RoomUser.Where(ru => ru.UserId == contactUserId && roomIds.Contains(ru.RoomId)).SingleOrDefaultAsync();

            return contactUserRooms == default ? -1L : contactUserRooms.RoomId; // not found
        }
    }
}
