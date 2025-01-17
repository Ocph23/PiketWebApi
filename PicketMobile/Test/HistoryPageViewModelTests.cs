using Moq;
using PicketMobile.Services;
using PicketMobile.Views;
using SharedModel.Requests;
using SharedModel.Responses;
using Xunit;

namespace PicketMobile.Tests
{
    public class HistoryPageViewModelTests
    {
        private readonly Mock<IPicketService> _picketServiceMock;
        private readonly HistoryPageViewModel _viewModel;

        public HistoryPageViewModelTests()
        {
            _picketServiceMock = new Mock<IPicketService>();
            ServiceHelper.RegisterService(_picketServiceMock.Object);
            _viewModel = new HistoryPageViewModel();
        }

        [Fact]
        public async Task LoadData_ShouldLoadDataCorrectly()
        {
            // Arrange
            var picketResponses = new List<PicketResponse>
            {
                new PicketResponse { Id = 1 },
                new PicketResponse { Id = 2 }
            };
            var paginationResponse = new PaginationResponse<PicketResponse>(picketResponses, 1,2,2);
            _picketServiceMock.Setup(s => s.Get(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(paginationResponse);

            // Act
            await _viewModel.LoadData();

            // Assert
            Assert.Equal(2, _viewModel.Datas.Count);
            Assert.Equal(1, _viewModel.CurrentPage);
            Assert.Equal(0, _viewModel.RemainingThreshold);
        }

        [Fact]
        public async Task LoadMoreData_ShouldLoadMoreDataCorrectly()
        {
            // Arrange
            var picketResponses = new List<PicketResponse>
            {
                new PicketResponse { Id = 3 },
                new PicketResponse { Id = 4 }
            };
            var paginationResponse = new PaginationResponse<PicketResponse>(picketResponses, 1, 2, 2);
            _picketServiceMock.Setup(s => s.Get(It.IsAny<PaginationRequest>()))
                .ReturnsAsync(paginationResponse);

            // Act
            await _viewModel.LoadMoreData();

            // Assert
            Assert.Equal(2, _viewModel.Datas.Count);
            Assert.Equal(2, _viewModel.CurrentPage);
            Assert.Equal(0, _viewModel.RemainingThreshold);
        }

        [Fact]
        public async Task LoadData_ShouldHandleException()
        {
            // Arrange
            _picketServiceMock.Setup(s => s.Get(It.IsAny<PaginationRequest>()))
                .ThrowsAsync(new System.Exception("Test exception"));

            // Act
            await _viewModel.LoadData();

            // Assert
            Assert.False(_viewModel.IsBusy);
        }

        [Fact]
        public async Task LoadMoreData_ShouldHandleException()
        {
            // Arrange
            _picketServiceMock.Setup(s => s.Get(It.IsAny<PaginationRequest>()))
                .ThrowsAsync(new System.Exception("Test exception"));

            // Act
            await _viewModel.LoadMoreData();

            // Assert
            Assert.False(_viewModel.IsBusyDataMore);
        }
    }
}
