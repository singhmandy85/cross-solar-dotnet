using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Exceptions;
using CrossSolar.Models;
using CrossSolar.Repository;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        public AnalyticsControllerTests()
        {
            _analyticsController = new AnalyticsController(_analyticsRepositoryMock.Object, _panelRepositoryMock.Object);
        }

        private readonly AnalyticsController _analyticsController;

        private readonly Mock<IPanelRepository> _panelRepositoryMock = new Mock<IPanelRepository>();
        private readonly Mock<IAnalyticsRepository> _analyticsRepositoryMock = new Mock<IAnalyticsRepository>();

        [Fact]
        public async Task Register_ShouldInsertAnalytics()
        {
            var panel = new OneHourElectricityModel
            {
                DateTime = DateTime.UtcNow,
                KiloWatt = 12
            };

            // Arrange

            // Act
            var result = await _analyticsController.Post(panelId: "1", value: panel);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Search_ReturnsList()
        {
            // Arrange
            var panelDbSetMock = Builder<Panel>.CreateListOfSize(3)
                .All()
                .With(c => c.Serial = Faker.RandomNumber.Next(1, 1).ToString())
                .Build().ToAsyncDbSetMock();
            _panelRepositoryMock.Setup(m => m.Query()).Returns(panelDbSetMock.Object);
             
            var analyticsDbSetMock = Builder<OneHourElectricity>.CreateListOfSize(3)
                .All()
                .With(c => c.PanelId = Faker.RandomNumber.Next(1, 1).ToString()).Build().ToAsyncDbSetMock();
            _analyticsRepositoryMock.Setup(m => m.Query()).Returns(analyticsDbSetMock.Object);

            // Act
            var result = await _analyticsController.Get("1");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as OneHourElectricityListModel;
            Assert.NotNull(content);

            Assert.Equal(3, content.OneHourElectricitys.Count());
        }

        [Fact]
        public void ConfigureServices_RegistersDependenciesCorrectly()
        {
            //  Arrange
            //  Setting up the stuff required for Configuration.GetConnectionString("DefaultConnection")
            Mock<IConfigurationSection> configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("CrossSolarDb");
            Mock<IConfiguration> configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);

            //  Act
            target.ConfigureServices(services);

            //  Assert

            var serviceProvider = services.BuildServiceProvider();

            Assert.NotNull(serviceProvider);
        }
    }
}