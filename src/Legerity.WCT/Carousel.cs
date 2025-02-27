namespace Legerity.Windows.Elements.WCT;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Legerity.Exceptions;
using Legerity.Windows.Elements.Core;
using Legerity.Windows.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

/// <summary>
/// Defines a <see cref="WindowsElement"/> wrapper for the Windows Community Toolkit Carousel control.
/// </summary>
public class Carousel : WindowsElementWrapper
{
    private readonly By carouselItemLocator = By.ClassName("CarouselItem");

    /// <summary>
    /// Initializes a new instance of the <see cref="Carousel"/> class.
    /// </summary>
    /// <param name="element">
    /// The <see cref="WindowsElement"/> reference.
    /// </param>
    public Carousel(WindowsElement element)
        : base(element)
    {
    }

    /// <summary>
    /// Gets the collection of items associated with the carousel.
    /// </summary>
    public virtual ReadOnlyCollection<AppiumWebElement> Items =>
        this.Element.FindElements(this.carouselItemLocator);

    /// <summary>
    /// Gets the element associated with the currently selected item.
    /// </summary>
    /// <exception cref="StaleElementReferenceException">Thrown when an element is no longer valid in the document DOM.</exception>
    public virtual AppiumWebElement SelectedItem => this.Items.FirstOrDefault(i => i.IsSelected());

    /// <summary>
    /// Gets the index of the element associated with the currently selected item.
    /// </summary>
    /// <exception cref="StaleElementReferenceException">Thrown when an element is no longer valid in the document DOM.</exception>
    public virtual int SelectedIndex => this.Items.IndexOf(this.SelectedItem);

    /// <summary>
    /// Allows conversion of a <see cref="WindowsElement"/> to the <see cref="ListView"/> without direct casting.
    /// </summary>
    /// <param name="element">
    /// The <see cref="WindowsElement"/>.
    /// </param>
    /// <returns>
    /// The <see cref="ListView"/>.
    /// </returns>
    public static implicit operator Carousel(WindowsElement element)
    {
        return new Carousel(element);
    }

    /// <summary>
    /// Allows conversion of a <see cref="AppiumWebElement"/> to the <see cref="ListView"/> without direct casting.
    /// </summary>
    /// <param name="element">
    /// The <see cref="AppiumWebElement"/>.
    /// </param>
    /// <returns>
    /// The <see cref="ListView"/>.
    /// </returns>
    public static implicit operator Carousel(AppiumWebElement element)
    {
        return new Carousel(element as WindowsElement);
    }

    /// <summary>
    /// Allows conversion of a <see cref="RemoteWebElement"/> to the <see cref="Carousel"/> without direct casting.
    /// </summary>
    /// <param name="element">
    /// The <see cref="RemoteWebElement"/>.
    /// </param>
    /// <returns>
    /// The <see cref="Carousel"/>.
    /// </returns>
    public static implicit operator Carousel(RemoteWebElement element)
    {
        return new Carousel(element as WindowsElement);
    }

    /// <summary>
    /// Clicks on an item in the carousel with the specified item name.
    /// </summary>
    /// <param name="name">
    /// The name of the item to click.
    /// </param>
    /// <exception cref="ElementsNotShownException">Thrown when no elements are shown for the expected locator.</exception>
    /// <exception cref="StaleElementReferenceException">Thrown when an element is no longer valid in the document DOM.</exception>
    /// <exception cref="InvalidElementStateException">Thrown when an element is not enabled.</exception>
    /// <exception cref="ElementNotVisibleException">Thrown when an element is not visible.</exception>
    public virtual void SelectItem(string name)
    {
        this.VerifyElementsShown(this.carouselItemLocator, TimeSpan.FromSeconds(2));

        int index = this.Items.IndexOf(this.Items.FirstOrDefault(element =>
            element.VerifyNameOrAutomationIdEquals(name)));

        this.SelectItemAtIndex(index);
    }

    /// <summary>
    /// Clicks on an item in the carousel with the specified partial item name.
    /// </summary>
    /// <param name="partialName">
    /// The partial name of the item to click.
    /// </param>
    /// <exception cref="ElementsNotShownException">Thrown when no elements are shown for the expected locator.</exception>
    /// <exception cref="StaleElementReferenceException">Thrown when an element is no longer valid in the document DOM.</exception>
    /// <exception cref="InvalidElementStateException">Thrown when an element is not enabled.</exception>
    /// <exception cref="ElementNotVisibleException">Thrown when an element is not visible.</exception>
    public virtual void SelectItemByPartialName(string partialName)
    {
        this.VerifyElementsShown(this.carouselItemLocator, TimeSpan.FromSeconds(2));

        int index = this.Items.IndexOf(this.Items.FirstOrDefault(element =>
            element.VerifyNameOrAutomationIdContains(partialName)));

        this.SelectItemAtIndex(index);
    }

    /// <summary>
    /// Clicks on an item in the carousel at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index of the item to click.
    /// </param>
    /// <exception cref="IndexOutOfRangeException">Thrown when an element is selected that is outside the range of items available.</exception>
    /// <exception cref="ElementsNotShownException">Thrown when no elements are shown for the expected locator.</exception>
    /// <exception cref="StaleElementReferenceException">Thrown when an element is no longer valid in the document DOM.</exception>
    /// <exception cref="InvalidElementStateException">Thrown when an element is not enabled.</exception>
    /// <exception cref="ElementNotVisibleException">Thrown when an element is not visible.</exception>
    public virtual void SelectItem(int index)
    {
        this.VerifyElementsShown(this.carouselItemLocator, TimeSpan.FromSeconds(2));

        if (index > this.Items.Count - 1)
        {
            throw new IndexOutOfRangeException(
                "Cannot select an element that is outside the range of items available.");
        }

        this.SelectItemAtIndex(index);
    }

    /// <exception cref="StaleElementReferenceException">Thrown when an element is no longer valid in the document DOM.</exception>
    /// <exception cref="InvalidElementStateException">Thrown when an element is not enabled.</exception>
    /// <exception cref="ElementNotVisibleException">Thrown when an element is not visible.</exception>
    private void SelectItemAtIndex(int index)
    {
        int selectedIndex = this.SelectedIndex;
        while (Math.Abs(index - selectedIndex) > double.Epsilon)
        {
            this.Element.SendKeys(selectedIndex < index ? Keys.ArrowRight : Keys.ArrowLeft);
            selectedIndex = this.SelectedIndex;
        }
    }
}