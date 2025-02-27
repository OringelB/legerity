namespace Legerity.Web.Tests.Pages;

using System;
using Elements.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

internal class DateInputPage : W3SchoolsBasePage
{
    public DateInputPage(RemoteWebDriver app) : base(app)
    {
    }

    public DateInput DateInput => this.FindElement(By.Id("birthday"));

    public DateInputPage SetBirthdayDate(DateTime date)
    {
        this.DateInput.SetDate(date);
        return this;
    }
}