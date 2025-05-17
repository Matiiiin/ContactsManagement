﻿using FluentAssertions;
using HtmlAgilityPack;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Net.Http;
using System.Net.Http.Json;
using AutoFixture;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using ServiceContracts.Persons;
using Services;

namespace CRUDTests.PersonTests;

public class PersonsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly IPersonsGetterService _personsGetterService;

    public PersonsControllerIntegrationTests(CustomWebApplicationFactory factory , ITestOutputHelper testOutputHelper)
    {
        _personsGetterService = factory.Services.GetRequiredService<IPersonsGetterService>();
        _testOutputHelper = testOutputHelper;
        _fixture = new Fixture();
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5114");
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnAllPersons()
    {
        //Arrange
        var requestUris = new [] {"/Persons/Index" , "/"};
        var responses = new List<HttpResponseMessage>();
        
        //Act
        foreach (var requestUri in requestUris)
        {
            var response = await _client.GetAsync(requestUri);
            responses.Add(response);
        }
        
        //Assert
        foreach (var response in responses)
        {
            response.EnsureSuccessStatusCode();
            var html = new HtmlDocument();
            html.LoadHtml(await response.Content.ReadAsStringAsync());
            var document = html.DocumentNode;
            document.Should().NotBeNull();
            var table = document.SelectSingleNode("//table[contains(@class, 'persons')]");
            document.SelectSingleNode("//h1[contains(text(), 'Persons')]").Should().NotBeNull();

            table.Should().NotBeNull();
        }
    }

    #endregion
    #region Create

    [Fact]
    public async Task Create_ShouldReturnViewWithInputs()
    {
        // Act
        var response = await _client.GetAsync("/Persons/Create");

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.Should().NotBeNull();
        document.SelectSingleNode("//select[@id=\"CountryID\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"PersonName\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Email\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"DateOfBirth\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Gender\"]").Should().NotBeNull();
        document.SelectSingleNode("//textarea[@id=\"Address\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"RecievesNewsLetters\"]").Should().NotBeNull();
        document.SelectSingleNode("//h2[contains(text(), 'Create Person')]").Should().NotBeNull();

    }

    #endregion
    
    #region Store

    [Fact]
    public async Task Store_ShouldRedirectToIndex_WhenModelIsValid()
    {
        // Arrange
        var validPerson = _fixture.Build<PersonAddRequest>().With(p=>p.Email , "test@gmail.com").Create();
        var formData = new MultipartFormDataContent
        {
            { new StringContent(validPerson.PersonName!), "PersonName" },
            { new StringContent(validPerson.Email!), "Email" },
            { new StringContent(validPerson.DateOfBirth.ToString()!), "DateOfBirth" },
            { new StringContent(validPerson.Gender.ToString()!), "Gender" },
            { new StringContent(validPerson.CountryID.ToString()!), "CountryID" },
            { new StringContent(validPerson.Address!), "Address" },
            { new StringContent(validPerson.RecievesNewsLetters.ToString()), "RecievesNewsLetters" }
        };
        // Act
        var response = await _client.PostAsync(
            "Persons/Store",
            formData 
        );

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.Should().NotBeNull();
        var table = document.SelectSingleNode("//table[contains(@class, 'persons')]");
        document.SelectSingleNode("//h1[contains(text(), 'Persons')]").Should().NotBeNull();

        table.Should().NotBeNull();
        (await _personsGetterService.GetAllPersons()).Where(p=>p.PersonName == validPerson.PersonName).Should().NotBeNull();
    }
        [Fact]
    public async Task Store_ShouldReturnViewWithErrors_WhenModelIsInvalid()
    {
        // Arrange
        var invalidPerson = _fixture.Build<PersonAddRequest>().With(p => p.Email , "asdasd").Create();
        var formData = new MultipartFormDataContent()
        {
            { new StringContent(invalidPerson.PersonName!), "PersonName" },
            { new StringContent(invalidPerson.Email!), "Email" },
            { new StringContent(invalidPerson.DateOfBirth.ToString()!), "DateOfBirth" },
            { new StringContent(invalidPerson.Gender.ToString()!), "Gender" },
            { new StringContent(invalidPerson.CountryID.ToString()!), "CountryID" },
            { new StringContent(invalidPerson.Address!), "Address" },
        };
        // Act
        var response = await _client.PostAsync("/Persons/Store", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        //Check create view is displayed
        document.Should().NotBeNull();
        document.SelectSingleNode("//select[@id=\"CountryID\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"PersonName\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Email\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"DateOfBirth\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"Gender\"]").Should().NotBeNull();
        document.SelectSingleNode("//textarea[@id=\"Address\"]").Should().NotBeNull();
        document.SelectSingleNode("//input[@id=\"RecievesNewsLetters\"]").Should().NotBeNull();
        document.SelectSingleNode("//h2[contains(text(), 'Create Person')]").Should().NotBeNull();

        response.EnsureSuccessStatusCode();

        // Check for validation error messages in the response
        var errorMessages = document.SelectNodes("//span[contains(@class, 'field-validation-error')]");
        errorMessages.Should().NotBeNull();
        errorMessages.Should().NotBeEmpty();
    }

    #endregion
    #region Edit

    [Fact]
    public async Task Edit_ShouldReturnViewWithPersonData_WhenPersonExists()
    {
        // Arrange
        var existigPerson =( await _personsGetterService.GetAllPersons()).FirstOrDefault();

        // Act
        var response = await _client.GetAsync($"/Persons/Edit/{existigPerson!.PersonID}");

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.SelectSingleNode("//input[@id='PersonName']").Should().NotBeNull();
        document.SelectSingleNode("//h2[contains(text(), 'Edit Person')]").Should().NotBeNull();
    }
    
   
    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentPersonID = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/Persons/Edit/{nonExistentPersonID}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        response.Content.Should().NotBeNull();
    }

    #endregion

    

    #region Update

    [Fact]
public async Task Update_ShouldRedirectToIndex_WhenModelIsValid()
{
    // Arrange
    var existingPerson = (await _personsGetterService.GetAllPersons()).FirstOrDefault();
    var validPersonUpdate = new
    {
        PersonID = existingPerson!.PersonID,
        PersonName = "Updated Name",
        Email = "updated.email@example.com",
        DateOfBirth = "1990-01-01",
        Gender = "Female",
        CountryID = existingPerson.CountryID,
        Address = "456 Updated St",
        RecievesNewsLetters = false
    };

    var editResponse = await _client.GetAsync($"/Persons/Edit/{validPersonUpdate.PersonID}");
    editResponse.EnsureSuccessStatusCode();
    var editHtml = new HtmlDocument();
    editHtml.LoadHtml(await editResponse.Content.ReadAsStringAsync());
    var antiForgeryToken = editHtml.DocumentNode
        .SelectSingleNode("//input[@name='__RequestVerificationToken']")
        ?.Attributes["value"]?.Value;

    var formData = new MultipartFormDataContent
    {
        { new StringContent(validPersonUpdate.PersonName), "PersonName" },
        { new StringContent(validPersonUpdate.PersonID.ToString()!), "PersonID" },
        { new StringContent(validPersonUpdate.Email), "Email" },
        { new StringContent(validPersonUpdate.DateOfBirth), "DateOfBirth" },
        { new StringContent(validPersonUpdate.Gender), "Gender" },
        { new StringContent(validPersonUpdate.CountryID.ToString()!), "CountryID" },
        { new StringContent(validPersonUpdate.Address), "Address" },
        { new StringContent(validPersonUpdate.RecievesNewsLetters.ToString()), "RecievesNewsLetters" },
        { new StringContent(antiForgeryToken!), "__RequestVerificationToken" }
    };

    // Act
    var response = await _client.PostAsync($"/Persons/Update/{validPersonUpdate.PersonID}", formData);

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

    var updatedPerson = await _personsGetterService.GetPersonByPersonID(validPersonUpdate.PersonID);
    updatedPerson.Should().NotBeNull();
    updatedPerson!.PersonName.Should().Be(validPersonUpdate.PersonName);
    updatedPerson.Email.Should().Be(validPersonUpdate.Email);
    updatedPerson.Gender.Should().Be(validPersonUpdate.Gender);
    updatedPerson.CountryID.Should().Be(validPersonUpdate.CountryID);
    updatedPerson.Address.Should().Be(validPersonUpdate.Address);
    updatedPerson.RecievesNewsLetters.Should().Be(validPersonUpdate.RecievesNewsLetters);

    var html = new HtmlDocument();
    html.LoadHtml(await response.Content.ReadAsStringAsync());
    var document = html.DocumentNode;
    document.Should().NotBeNull();
    var table = document.SelectSingleNode("//table[contains(@class, 'persons')]");
    document.SelectSingleNode("//h1[contains(text(), 'Persons')]").Should().NotBeNull();
    table.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ShouldReturnViewWithErrors_WhenModelIsInvalid()
    {
        // Arrange
        var existingPerson = (await _personsGetterService.GetAllPersons()).FirstOrDefault();
        var invalidPersonUpdate = _fixture.Build<PersonUpdateRequest>().With(p=>p.Email , "asdad").With(p=>p.PersonID , existingPerson.PersonID).Create();
        var editResponse = await _client.GetAsync($"/Persons/Edit/{invalidPersonUpdate.PersonID}");
        editResponse.EnsureSuccessStatusCode();
        var editHtml = new HtmlDocument();
        editHtml.LoadHtml(await editResponse.Content.ReadAsStringAsync());
        var antiForgeryToken = editHtml.DocumentNode
            .SelectSingleNode("//input[@name='__RequestVerificationToken']")
            ?.Attributes["value"]?.Value;

        var formData = new MultipartFormDataContent()
        {
            { new StringContent(invalidPersonUpdate.PersonID.ToString()!), "PersonID" },
            { new StringContent(invalidPersonUpdate.PersonName!), "PersonName" },
            { new StringContent(invalidPersonUpdate.CountryID.ToString()!), "CountryID" },
            { new StringContent(invalidPersonUpdate.Email!), "Email" },
            { new StringContent(invalidPersonUpdate.Address!), "Address" },
            { new StringContent(invalidPersonUpdate.RecievesNewsLetters.ToString()!), "RecievesNewsLetters" },
            { new StringContent(invalidPersonUpdate.Gender.ToString()!), "Gender" },
            { new StringContent(invalidPersonUpdate.DateOfBirth.ToString()!), "DateOfBirth" },
            { new StringContent(antiForgeryToken!), "__RequestVerificationToken" }
        };
        // Act
        var response = await _client.PostAsync($"/Persons/Update/{invalidPersonUpdate.PersonID}",formData);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        var errorMessages = document.SelectNodes("//span[contains(@class, 'field-validation-error')]");
        errorMessages.Should().NotBeNullOrEmpty();
    }
    #endregion
    #region Delete

    [Fact]
    public async Task Delete_ShouldReturnViewWithPersonData_WhenPersonExists()
    {
        // Arrange
        var existingPerson =(await _personsGetterService.GetAllPersons()).FirstOrDefault(); // Replace with an existing person ID

        // Act
        var response = await _client.GetAsync($"/Persons/Delete?personID={existingPerson!.PersonID}");

        // Assert
        response.EnsureSuccessStatusCode();
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.SelectSingleNode("//h2[contains(text(), 'Delete Person')]").Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenPersonIDIsInvalid()
    {
        // Act
        var response = await _client.GetAsync("/Persons/Delete?personID=");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    #endregion
    
    
    #region SubmitDelete

    [Fact]
    public async Task SubmitDelete_ShouldRedirectToIndex_WhenPersonExists()
    {
        // Arrange
        var existingPerson =(await _personsGetterService.GetAllPersons()).FirstOrDefault();
        var editResponse = await _client.GetAsync($"/Persons/Edit/{existingPerson!.PersonID}");
        editResponse.EnsureSuccessStatusCode();
        var editHtml = new HtmlDocument();
        editHtml.LoadHtml(await editResponse.Content.ReadAsStringAsync());
        var antiForgeryToken = editHtml.DocumentNode
            .SelectSingleNode("//input[@name='__RequestVerificationToken']")
            ?.Attributes["value"]?.Value;
        var formData = new MultipartFormDataContent()
        {
            { new StringContent(existingPerson!.PersonID.ToString()!), "PersonID" },
            { new StringContent(antiForgeryToken!), "__RequestVerificationToken" }
        };
        
        // Act
        var response = await _client.PostAsync("/Persons/SubmitDelete", formData);

        // Assert
        var allPersons = await _personsGetterService.GetAllPersons();
        allPersons.Should().NotContain(existingPerson);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var html = new HtmlDocument();
        html.LoadHtml(await response.Content.ReadAsStringAsync());
        var document = html.DocumentNode;
        document.Should().NotBeNull();
        var table = document.SelectSingleNode("//table[contains(@class, 'persons')]");
        document.SelectSingleNode("//h1[contains(text(), 'Persons')]").Should().NotBeNull();
        table.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitDelete_ShouldReturnNotFound_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentPersonID = Guid.NewGuid();
        var existingPerson =(await _personsGetterService.GetAllPersons()).FirstOrDefault();
        var editResponse = await _client.GetAsync($"/Persons/Edit/{existingPerson!.PersonID}");
        editResponse.EnsureSuccessStatusCode();
        var editHtml = new HtmlDocument();
        editHtml.LoadHtml(await editResponse.Content.ReadAsStringAsync());
        var antiForgeryToken = editHtml.DocumentNode
            .SelectSingleNode("//input[@name='__RequestVerificationToken']")
            ?.Attributes["value"]?.Value;
        var formData = new MultipartFormDataContent()
        {
            { new StringContent(nonExistentPersonID.ToString()!), "PersonID" },
            { new StringContent(antiForgeryToken!), "__RequestVerificationToken" },
        };
        // Act
        var response = await _client.PostAsync("/Persons/SubmitDelete", formData);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        
    }

    #endregion
}