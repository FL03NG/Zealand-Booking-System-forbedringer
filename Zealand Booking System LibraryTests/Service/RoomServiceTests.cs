using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zealand_Booking_System_Library.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Service;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Models;
using Moq;
namespace Zealand_Booking_System_Library.Service.Tests
{
    /*
     * RoomServiceTests
     *
     * Purpose:
     * This class validates the behavior of RoomService by testing its business
     * rules and interaction with the repository layer. Mock objects are used to
     * ensure tests focus solely on service logic, not database or infrastructure
     * concerns.
     */
    [TestClass()]
    public class RoomServiceTests
    {
        //--------------------------------------AddRoom---------------------
        [TestMethod]
        public void AddRoom_ShouldCallRepositoryAddRoom_Once()
        {
            // Arrange
            // A valid room should result in exactly one repository call,
            // proving that the service correctly delegates persistence
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomName = "TestLokale",
                RoomLocation = "A123",
                RoomDescription = "Test description",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act
            service.AddRoom(room);

            // Assert
            mockRepo.Verify(r => r.AddRoom(room), Times.Once);
        }

        [TestMethod]
        public void AddRoom_NullRoom_ShouldThrowArgumentNullException()
        {
            // Arrange
            // Invalid domain data should be rejected early to prevent
            // corrupt data from reaching the repository
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            Room room = new Room
            {
                RoomName = null, // violates required business rules
                RoomLocation = "A123",
                RoomDescription = "Test",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => service.AddRoom(room));
        }

        //-------------------------DeleteRoom-------------------------
        [TestMethod]
        public void DeleteRoom_ValidId_ShouldCallRepositoryOnce()
        {
            // Arrange
            // A valid identifier should always be forwarded to the repository,
            // confirming that the service does not block legitimate operations
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            int roomId = 1;

            // Act
            service.DeleteRoom(roomId);

            // Assert
            mockRepo.Verify(r => r.DeleteRoom(roomId), Times.Once);
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_Zero_ShouldThrowArgumentException()
        {
            // Arrange
            // Defensive validation ensures meaningless identifiers
            // never trigger repository operations
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => service.DeleteRoom(0));
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_Negative_ShouldThrowArgumentException()
        {
            // Arrange
            // Negative identifiers violate domain rules and must fail fast
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => service.DeleteRoom(-5));
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_MinValue_ShouldThrowArgumentException()
        {
            // Arrange
            // Extreme edge cases are tested to ensure consistent validation behavior
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => service.DeleteRoom(int.MinValue));
        }

        [TestMethod]
        public void DeleteRoom_Valid_LargeId_ShouldCallRepository()
        {
            // Arrange
            // The service should not impose artificial limits beyond domain rules
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            int roomId = int.MaxValue;

            // Act
            service.DeleteRoom(roomId);

            // Assert
            mockRepo.Verify(r => r.DeleteRoom(roomId), Times.Once);
        }
        //-----------------------------UpdateRoom-----------------
        [TestMethod]
        public void UpdateRoom_ValidRoom_ShouldCallRepositoryOnce()
        {
            // Arrange
            // When validation passes, the service should delegate
            // the update responsibility exactly once
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomDescription = "Desc",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act
            service.UpdateRoom(room);

            // Assert
            mockRepo.Verify(r => r.UpdateRoom(room), Times.Once);
        }

        
        [TestMethod]
        public void UpdateRoom_NullRoom_ShouldThrowArgumentNullException()
        {
            // Arrange
            // A null room simulates a programming error
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            // Act & Assert
            // The service must fail fast when receiving a null reference
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                service.UpdateRoom(null);
            });
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoomId_ShouldThrowArgumentException()
        {
            // Arrange
            // A room with an invalid identifier violates domain rules
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 0,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };
            // Act & Assert
            // Invalid identifiers must prevent update operations
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_EmptyRoomName_ShouldThrowArgumentException()
        {
            // Arrange
            // An empty room name represents incomplete domain data
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };
            // Act & Assert
            // Required fields must contain meaningful values
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_NullRoomName_ShouldThrowArgumentException()
        {
            // Arrange
            // A null name breaks mandatory domain constraints
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = null,
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };
            // Act & Assert
            // Null values for required fields must be rejected
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_EmptyRoomLocation_ShouldThrowArgumentException()
        {
            // Arrange
            // An empty location prevents meaningful identification of the room
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "",
                RoomType = RoomType.ClassRoom
            };
            // Act & Assert
            // Mandatory location data must be present
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_NullRoomLocation_ShouldThrowArgumentException()
        {
            // Arrange
            // A null location indicates incomplete input data
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = null,
                RoomType = RoomType.ClassRoom
            };
            // Act & Assert
            // The service must enforce required field validation
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoomType_ShouldThrowArgumentException()
        {
            // Arrange
            // An unsupported enum value violates domain constraints
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = (RoomType)999
            };
            // Act & Assert
            // Only defined enum values should be accepted
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoom_ShouldNotCallRepository()
        {
            // Arrange
            // Invalid data is prepared to ensure the repository is never reached
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = -1,                 
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };
            // Act
            try { service.UpdateRoom(room); }
            catch { /* exception is expected */ }
            // Assert
            // Repository interaction must not occur when validation fails
            mockRepo.Verify(r => r.UpdateRoom(It.IsAny<Room>()), Times.Never);
        }
    }
}