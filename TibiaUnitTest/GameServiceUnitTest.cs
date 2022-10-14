using Moq;
using Shouldly;
using Tibia.Adventures;
using Tibia.Enteties.Interfaces;
using Tibia.Enteties.Models;
using Tibia.Game;
using Tibia.Utilities.Interfaces;
using Xunit;

namespace TibiaUnitTest
{
    public class GameServiceUnitTest
    {
        private GameService gameService;

        private Mock<IAdventureService> mockAdventureService = new Mock<IAdventureService>();
        private Mock<ICharacterService> mockCharacterService = new Mock<ICharacterService>();
        private Mock<IMessageHandler> mockMessageHandler = new Mock<IMessageHandler>();

        public GameServiceUnitTest()
        {
            gameService = new GameService(mockAdventureService.Object, mockCharacterService.Object, mockMessageHandler.Object);
        }

        [Fact]
        public void Method_Should_Return_Exception_When_Service_Throws_Exception()
        {
            //Arrange
            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Throws(new Exception());
            //Act
            Should.Throw<Exception>(() => gameService.StartGame());
            //Assert
        }

        [Fact]
        public void Method_Should_Return_False_When_No_Character_In_Range()
        {
            //Arrange
            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure{ Title = "Test Title", Description = "Test Descri" });
            mockCharacterService.Setup(_ => _.GetCharacterInRange(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Character>());
            //Act
            var methodReturn = gameService.StartGame();
            
            //Assert
            methodReturn.ShouldBeFalse();
        }

        [Fact]
        public void Method_Should_Return_True_When_Character_Is_In_Range()
        {
            //Arrange
            var charList = new List<Character>
            {
                new Character { Name = "Frank" },
                new Character { Name = "Sven" }
            };

            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "Test Title", Description = "Test Descri" });
            mockCharacterService.Setup(_ => _.GetCharacterInRange(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).Returns(charList);
            mockMessageHandler.Setup(_ => _.Read()).Returns("0");
            //Act
            var methodReturn = gameService.StartGame();

            //Assert
            methodReturn.ShouldBeTrue();
        }

        [Fact]
        public void Method_Should_Throw_Exception_When_CharacterInput_Not_A_Number()
        {
            //Arrange
            var charList = new List<Character>
            {
                new Character { Name = "Frank" },
                new Character { Name = "Sven" }
            };

            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "Test Title", Description = "Test Descri" });
            mockCharacterService.Setup(_ => _.GetCharacterInRange(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).Returns(charList);
            mockMessageHandler.Setup(_ => _.Read()).Returns("G");

            //Act
            Should.Throw<Exception>(() => gameService.StartGame());

            //Assert

        }
    }
}