using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Enums;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class RoomTests
    {
        // Вспомогательный метод для создания тестовой комнаты
        private Room CreateTestRoom(
            int id = 1,
            string roomNumber = "101",
            decimal area = 20,
            int floorNumber = 2,
            FinishingType finishingType = FinishingType.Standard,
            bool hasPhone = false)
        {
            return new Room(id, roomNumber, area, floorNumber, finishingType, hasPhone);
        }

        [TestMethod]
        public void Constructor_ValidData_CreatesRoom()
        {
            // Arrange & Act
            var room = CreateTestRoom();

            // Assert
            Assert.AreEqual("101", room.RoomNumber);
            Assert.AreEqual(20, room.Area);
            Assert.AreEqual(2, room.FloorNumber);
            Assert.AreEqual(FinishingType.Standard, room.FinishingType);
            Assert.IsFalse(room.HasPhone);
            Assert.IsFalse(room.IsRented);
            Assert.IsNull(room.CurrentAgreementId);
        }
    }
}
