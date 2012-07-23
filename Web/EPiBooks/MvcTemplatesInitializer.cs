﻿using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;

namespace EPiBooks {
    [ServiceConfiguration(typeof(IViewTemplateModelRegistrator))]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class MvcTemplatesInitializer : IConfigurableModule, IViewTemplateModelRegistrator {
        public void ConfigureContainer(ServiceConfigurationContext context) {
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.Container));
        }

        public void Preload(string[] parameters) {

        }

        public void Initialize(InitializationEngine context) {

        }

        public void Uninitialize(InitializationEngine context) {

        }

        public void Register(TemplateModelCollection viewTemplateModelRegistrator) {
            viewTemplateModelRegistrator.Add(typeof(PageData),
                new EPiServer.DataAbstraction.TemplateModel() {
                    Name = "PageTeaser",
                    Description = "Displays a teaser of a page.",
                    Path = "~/Views/Shared/PageTeaser.cshtml",
                    Inherited = true
                }
            );
        }
    }
}