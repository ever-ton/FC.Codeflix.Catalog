using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryFixture;

    public CategoryTest(CategoryTestFixture categoryFixture)
        => _categoryFixture = categoryFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);


        //Assert 
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
        (category.IsActive).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);


        //Assert 
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
        (category.IsActive).Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();

        //Act
        Action action =
            () => new DomainEntity.Category(name!, validCategory.Description);

        //Assert
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();

        //Act
        Action action =
            () => new DomainEntity.Category(validCategory.Name, null!);

        //Assert
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ca")]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();

        //Act
        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        //Assert
        action.Should().Throw<EntityValidationException>()
           .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();
        var invalidName = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());

        //Act
        Action action =
            () => new DomainEntity.Category(invalidName, validCategory.Description);

        //Assert
        action.Should().Throw<EntityValidationException>()
          .WithMessage("Name should be less or equals 255 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();
        var invalidDescription = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        
        //Act
        Action action =
            () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        //Assert
        action.Should().Throw<EntityValidationException>()
          .WithMessage("Description should be less or equals 10_000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        //Assert 
        (category.IsActive).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
        category.Deactivate();

        //Assert 
        (category.IsActive).Should().BeFalse();
    }
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();
        var categoryWithNewValues = _categoryFixture.GetValidCategory();

        //Act
        validCategory.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);

        //Assert
        validCategory.Name.Should().Be(categoryWithNewValues.Name);
        validCategory.Description.Should().Be(categoryWithNewValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();
        var newName = _categoryFixture.GetValidCategoryName();
        var currentDescription = validCategory.Description;

        //Act
        validCategory.Update(newName);

        //Assert
        validCategory.Name.Should().Be(newName);
        validCategory.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        //Arrange
        var validCategory = _categoryFixture.GetValidCategory();

        //Act
        Action action =
            () => validCategory.Update(name!);

        //Assert
        action.Should().Throw<EntityValidationException>()
         .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNameWithLessThan3Characters), parameters: 10)]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        //Arrange
        var category = _categoryFixture.GetValidCategory();

        //Act
        Action action =
            () => category.Update(invalidName);

        //Assert
        action.Should().Throw<EntityValidationException>()
         .WithMessage("Name should be at least 3 characters long");
    }

    public static IEnumerable<object[]> GetNameWithLessThan3Characters(int numberOfTests = 6)
    {
        var fixture = new CategoryTestFixture();

        for(int i = 0;i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            yield return new object[] { 
                fixture.GetValidCategoryName()[..(isOdd ? 1 : 2)] };
        }
    }


    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        //Arrange
        var category = _categoryFixture.GetValidCategory();
        var invalidName = _categoryFixture.Faker.Lorem.Letter(256);

        //Act
        Action action =
            () => category.Update(invalidName);

        //Assert
        action.Should().Throw<EntityValidationException>()
        .WithMessage("Name should be less or equals 255 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        //Arrange
        var category = _categoryFixture.GetValidCategory();
        var invalidDescription = _categoryFixture.Faker.Commerce.ProductDescription();

        while (invalidDescription.Length <= 10_000)
            invalidDescription = $"{invalidDescription} {_categoryFixture.Faker.Commerce.ProductDescription()}";

        //Act
        Action action =
            () => category.Update("Category Name", invalidDescription);

        //Assert
        action.Should().Throw<EntityValidationException>()
       .WithMessage("Description should be less or equals 10_000 characters long");
    }
}

