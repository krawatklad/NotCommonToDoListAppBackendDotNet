using Application.Common.Enums;
using Application.TaskItems.Interfaces;
using Infrastructure.TaskItems;
using Moq;

namespace Infrastructure.Tests.TaskItems;

public class TaskItemExportStrategyFactoryTests
{
    private readonly TaskItemExportStrategyFactory _factory;
    private readonly Mock<ITaskItemExportPdf> _pdfMock;
    private readonly Mock<ITaskItemExportXlsx> _xlsxMock;

    public TaskItemExportStrategyFactoryTests()
    {
        _xlsxMock = new Mock<ITaskItemExportXlsx>();
        _pdfMock = new Mock<ITaskItemExportPdf>();
        _factory = new TaskItemExportStrategyFactory(_xlsxMock.Object, _pdfMock.Object);
    }

    [Fact]
    public void GetStrategy_WhenFormatIsPdf_ShouldReturnPdfStrategy()
    {
        // Arrange
        const ExportFormat format = ExportFormat.Pdf;
        
        // Act
        var result = _factory.GetStrategy(format);

        // Assert
        Assert.Same(_pdfMock.Object, result);
    }

    [Fact]
    public void GetStrategy_WhenFormatIsXlsx_ShouldReturnXlsxStrategy()
    {
        // Arrange
        const ExportFormat format = ExportFormat.Xlsx;
        
        // Act
        var result = _factory.GetStrategy(format);

        // Assert
        Assert.Same(_xlsxMock.Object, result);
    }

    [Fact]
    public void GetStrategy_WhenFormatIsInvalid_ShouldThrowNotImplementedException()
    {
        // Arrange
        const ExportFormat format = (ExportFormat)999;
        
        // Act
        var act = () => _factory.GetStrategy(format);

        // Assert
        Assert.Throws<NotImplementedException>(act);
    }
}
