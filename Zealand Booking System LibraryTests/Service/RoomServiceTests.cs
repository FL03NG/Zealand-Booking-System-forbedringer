using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zealand_Booking_System_Library.Service;
using System;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Models;
using Moq;

namespace Zealand_Booking_System_Library.Service.Tests
{
    [TestClass()]
    public class RoomServiceTests
    {
        //--------------------------------------AddRoom---------------------
        [TestMethod]
        public void AddRoom_ShouldCallRepositoryAddRoom_Once()
        {
            // Arrange
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
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act + Assert
            // IMPORTANT: This must pass null to match RoomService (room == null)
            try
            {
                service.AddRoom(null);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("room", ex.ParamName);
            }
        }

        //-------------------------DeleteRoom-------------------------
        [TestMethod]
        public void DeleteRoom_ValidId_ShouldCallRepositoryOnce()
        {
            // Arrange
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
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act + Assert
            try
            {
                service.DeleteRoom(0);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Invalid room ID.", ex.Message);
            }
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_Negative_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act + Assert
            try
            {
                service.DeleteRoom(-5);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Invalid room ID.", ex.Message);
            }
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_MinValue_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act + Assert
            try
            {
                service.DeleteRoom(int.MinValue);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Invalid room ID.", ex.Message);
            }
        }

        [TestMethod]
        public void DeleteRoom_Valid_LargeId_ShouldCallRepository()
        {
            // Arrange
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
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act + Assert
            try
            {
                service.UpdateRoom(null);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("room", ex.ParamName);
            }
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoomId_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 0,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            // Act + Assert
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Invalid room ID.", ex.Message);
            }
        }

        [TestMethod]
        public void UpdateRoom_EmptyRoomName_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            // Act + Assert
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Room name is required.", ex.Message);
            }
        }

        [TestMethod]
        public void UpdateRoom_NullRoomName_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = null,
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            // Act + Assert
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Room name is required.", ex.Message);
            }
        }

        [TestMethod]
        public void UpdateRoom_EmptyRoomLocation_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "",
                RoomType = RoomType.ClassRoom
            };

            // Act + Assert
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Room location is required.", ex.Message);
            }
        }

        [TestMethod]
        public void UpdateRoom_NullRoomLocation_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = null,
                RoomType = RoomType.ClassRoom
            };

            // Act + Assert
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Room location is required.", ex.Message);
            }
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoomType_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = (RoomType)999
            };

            // Act + Assert
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Invalid room type.", ex.Message);
            }
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoom_ShouldNotCallRepository()
        {
            // Arrange
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
            try
            {
                service.UpdateRoom(room);
                Assert.Fail("Expected an exception, but none was thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid room ID.", ex.Message);
            }

            // Assert
            mockRepo.Verify(r => r.UpdateRoom(It.IsAny<Room>()), Times.Never);
        }
    }
}
