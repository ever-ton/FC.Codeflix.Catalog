﻿using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task Getcategory()
    {
        //Arrange
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleCategory = _fixture.GetValidCategory();

        repositoryMock
            .Setup(x => x.GetAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var input = new UseCase.GetCategoryInput(exampleCategory.Id);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        //Act
        var output = await useCase.Handle(input, CancellationToken.None);

        //Assert
        repositoryMock.Verify(r => r.GetAsync(
           It.IsAny<Guid>(),
           It.IsAny<CancellationToken>()
           ), Times.Once);

        output.Should().NotBeNull();
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.Id.Should().Be(exampleCategory.Id);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExist))]
    [Trait("Application", "GetCategory - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesntExist()
    {
        //Arrange
        var repositoryMock = _fixture.GetRepositoryMock();
        var fakeCategoryId = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.GetAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
            new NotFoundException($"Category '{fakeCategoryId}' not found"));

        var input = new UseCase.GetCategoryInput(fakeCategoryId);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        //Act
        var task = async () 
            => await useCase.Handle(input, CancellationToken.None);

        //Assert
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(r => r.GetAsync(
           It.IsAny<Guid>(),
           It.IsAny<CancellationToken>()
           ), Times.Once);
    }
}
