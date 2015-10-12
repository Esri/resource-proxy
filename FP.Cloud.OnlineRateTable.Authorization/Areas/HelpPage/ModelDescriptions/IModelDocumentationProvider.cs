using System;
using System.Reflection;

namespace FP.Cloud.OnlineRateTable.Authorization.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}