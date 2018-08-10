using CrossSolar.Domain;
using CrossSolar.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class PanelRepositoryTests : IDisposable
    {

        private CrossSolarDbContext _testDBContext;
        private readonly IPanelRepository _panelRepository;

        public PanelRepositoryTests()
        {
            _testDBContext = InMemoryContext();
            _panelRepository = new PanelRepository(_testDBContext);
        }

        private CrossSolarDbContext InMemoryContext()
        {
            var options = new DbContextOptionsBuilder<CrossSolarDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new CrossSolarDbContext(options);

            return context;
        }

        [Fact]
        public async void RegisterNewPanels()
        {
            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };

            await _panelRepository.InsertAsync(panel);
        }

        [Fact]
        public async void QueryPanles()
        {
            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };

            _testDBContext.Add(panel);
            await _testDBContext.SaveChangesAsync();

            var result = await _panelRepository.Query().ToListAsync();

            Assert.True(result.Count > 0);
        }

        public void Dispose()
        {
            if (_testDBContext!=null)
            {
                _testDBContext.Dispose();
            }
        }
    }
}
