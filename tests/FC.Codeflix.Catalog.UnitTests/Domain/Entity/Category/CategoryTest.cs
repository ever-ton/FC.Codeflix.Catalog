using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using System;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

public class CategoryTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var dateTimeAfter = DateTime.Now;


        //Assert 
        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt < dateTimeAfter).Should().BeTrue();
        (category.IsActive).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;


        //Assert 
        category.Should().NotBeNull();
        category.Name.Should().Be(validData.Name);
        category.Description.Should().Be(validData.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default);
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt < dateTimeAfter).Should().BeTrue();
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
        Action action =
            () => new DomainEntity.Category(name!, "Category description");

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        //Arrange
        Action action =
            () => new DomainEntity.Category("Category Name", null!);

        //Act and Assert
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
        Action action =
            () => new DomainEntity.Category(invalidName, "Category Ok Description");

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
           .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        //Arrange
        var invalidName = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        Action action =
            () => new DomainEntity.Category(invalidName, "Category Ok Description");

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
          .WithMessage("Name should be less or equals 255 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        //Arrange
        var invalidDescription = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        Action action =
            () => new DomainEntity.Category("Category Name", invalidDescription);

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
          .WithMessage("Description should be less or equals 10_000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        category.Activate();

        //Assert 
        (category.IsActive).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        category.Deactivate();

        //Assert 
        (category.IsActive).Should().BeFalse();
    }
    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        //Arrange
        var category = new DomainEntity.Category("Category name", "Category description");
        var newValues = new { Name = "New Name", Description = "New Description" };

        //Act
        category.Update(newValues.Name, newValues.Description);

        //Assert
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        //Arrange
        var category = new DomainEntity.Category("Category name", "Category description");
        var newValues = new { Name = "New Name" };
        var currentDescription = category.Description;

        //Act
        category.Update(newValues.Name);

        //Assert
        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        //Arrange
        var category = new DomainEntity.Category("Category name", "Category description");
        Action action =
            () => category.Update(name!);

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
         .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("a")]
    [InlineData("ca")]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        //Arrange
        var category = new DomainEntity.Category("Category name", "Category description");
        Action action =
            () => category.Update(invalidName);

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
         .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        //Arrange
        var category = new DomainEntity.Category("Category name", "Category description");
        var invalidName = string.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        Action action =
            () => category.Update(invalidName);

        //Act
        action.Should().Throw<EntityValidationException>()
        .WithMessage("Name should be less or equals 255 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        //Arrange
        var category = new DomainEntity.Category("Category name", "Category description");
        var invalidDescription = string.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());

        Action action =
            () => category.Update("Category Name", invalidDescription);

        //Act and Assert
        action.Should().Throw<EntityValidationException>()
       .WithMessage("Description should be less or equals 10_000 characters long");
    }
}

