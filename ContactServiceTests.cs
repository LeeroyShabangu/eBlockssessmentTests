using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using eBlockAssessent.Data;
using eBlockAssessent.Models;
using eBlockAssessent.Services;

namespace eBlockssessent.Tests
{
    public class ContactServiceTests
    {
        private readonly ContactService _contactService;
        private readonly Mock<ContactDbContext> _mockContext;
        public ContactServiceTests()
        {
            _mockContext = new Mock<ContactDbContext>(new DbContextOptions<ContactDbContext>());
            _contactService = new ContactService(_mockContext.Object);
        }

        [Fact]
        public async Task GetContactsAsync_ReturnsListOfContacts()
        {
            var contacts = new List<Contact>
            {
                new Contact { Id = 1, Name = "Lee", Surname = "Shaba", ContactInfo = "lee@test.com" },
                new Contact { Id = 2, Name = "muzi", Surname = "MK", ContactInfo = "muzi@test.com" }
            };

            var mockDbSet = new Mock<DbSet<Contact>>();
            mockDbSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(contacts.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(contacts.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(contacts.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(contacts.AsQueryable().GetEnumerator());

            _mockContext.Setup(c => c.Contacts).Returns(mockDbSet.Object);

            var result = await _contactService.GetContactsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddContactAsync_AddsContactToContext()
        {
            var newContact = new Contact { Id = 3, Name = "test", Surname = "testsurname", ContactInfo = "test" };

            await _contactService.AddContactAsync(newContact);

            _mockContext.Verify(c => c.Contacts.Add(newContact), Times.Once);
        }

        [Fact]
        public async Task UpdateContactAsync_ReturnsTrueAndUpdateContact()
        {
            var existingContact = new Contact { Id = 4, Name = "test", Surname = "testwse", ContactInfo = "testc" };
            _mockContext.Setup(c => c.Contacts.FindAsync(existingContact.Id)).ReturnsAsync(existingContact);

            var updatedContact = new Contact { Id = 4, Name = "Updated", Surname = "Name", ContactInfo = "Test" };
            var result = await _contactService.UpdateContactAsync(existingContact.Id, updatedContact);

            Assert.True(result);
            Assert.Equal("Updated", existingContact.Name);
            Assert.Equal("Name", existingContact.Surname);
            Assert.Equal("Test", existingContact.ContactInfo);
        }

        [Fact]
        public async Task DeleteContactAsync_ReturnsTrueAndRemovesContact()
        {
            var existingContact = new Contact { Id = 5, Name = "Lee", Surname = "Muzi", ContactInfo = "Leemuzi@" };
            _mockContext.Setup(c => c.Contacts.FindAsync(existingContact.Id)).ReturnsAsync(existingContact);

            var result = await _contactService.DeleteContactAsync(existingContact.Id);

            Assert.True(result);
            _mockContext.Verify(c => c.Contacts.Remove(existingContact), Times.Once);
        }
    }
}
