using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture)
        => _fixture = fixture;


    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        //Arrange
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var input = _fixture.GetInput();

        //Act 
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

        unitOfWorkMock.Verify(
            unitOfWorkMock => unitOfWorkMock.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        //Assert
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithOnlyName()
    {
        //Arrange
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var input = new CreateCategoryInput(_fixture.GetValidCategoryName());

        //Act 
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

        unitOfWorkMock.Verify(
            unitOfWorkMock => unitOfWorkMock.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        //Assert
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().Be(true);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        //Arrange
        var repositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object);

        var input = new CreateCategoryInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription());

        //Act 
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

        unitOfWorkMock.Verify(
            unitOfWorkMock => unitOfWorkMock.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        //Assert
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(true);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }


    [Theory(DisplayName = nameof(ThrowWhenCannotInstantiateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(CreateCategoryDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(CreateCategoryDataGenerator))]
    public async void ThrowWhenCannotInstantiateCategory(CreateCategoryInput input,
        string exceptionMessage)
    {
        //Arrange        
        var useCase = new UseCases.CreateCategory(
            _fixture.GetCategoryRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object);

        //Act 
        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);


        //Assert
        await task.Should()
             .ThrowAsync<EntityValidationException>()
             .WithMessage(exceptionMessage);
    }

    
}
