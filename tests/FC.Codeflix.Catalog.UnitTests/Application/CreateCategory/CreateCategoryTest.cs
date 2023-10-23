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


    [Theory(DisplayName = nameof(ThrowWhenInstantiateAggregate))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async void ThrowWhenInstantiateAggregate(CreateCategoryInput input,
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

    private static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();

        //name at least 3 characters
        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name =
            invalidInputShortName.Name[..2];

        invalidInputsList.Add(new object[]
        {
            invalidInputShortName,
            "Name should be at least 3 characters long"
        });

        //name could not be more than 255 characters
        var invalidInputTooLongName = fixture.GetInput();

        var tooLongNameForCategory = fixture.Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {fixture.Faker.Commerce.ProductName}";

        invalidInputTooLongName.Name = tooLongNameForCategory;

        invalidInputsList.Add(new object[]
        {
            invalidInputTooLongName,
            "Name should be less or equals 255 characters long"
        });

        //description could not be null
        var invalidInputDescriptionNull = fixture.GetInput();
        invalidInputDescriptionNull.Description = null;

        invalidInputsList.Add(new object[]
        {
            invalidInputDescriptionNull,
            "Description should not be null"
        });
        
        //description shoul be greater than 10_000 characters
        var invalidInputTooLongDescription = fixture.GetInput();
        var tooLongDescriptionForCategory = fixture.Faker.Commerce.ProductDescription();

        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {fixture.Faker.Commerce.ProductDescription()}";

        invalidInputTooLongDescription.Description = tooLongDescriptionForCategory;

        invalidInputsList.Add(new object[]
        {
            invalidInputTooLongDescription,
            "Description should be less or equals 10000 characters long"
        });

        return invalidInputsList;
    }
}
