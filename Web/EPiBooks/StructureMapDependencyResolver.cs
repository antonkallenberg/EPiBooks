/* 
 * StructureMapDependencyResolver 
 * Constructed from code posted by Eric Lee on:
 * http://blogs.msdn.com/b/elee/archive/2010/11/19/asp-net-mvc-3-idependencyresolver-and-structuremap.aspx
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace EPiBooks {
    public class StructureMapDependencyResolver : IDependencyResolver {
        private readonly IContainer container;

        public StructureMapDependencyResolver(IContainer container) {
            this.container = container;
        }

        public object GetService(Type serviceType) {
            if (serviceType.IsClass || serviceType.IsAbstract) {
                return GetConcreteService(serviceType);
            }
            return GetInterfaceService(serviceType);
        }

        private object GetConcreteService(Type serviceType) {
            try {
                // Can't use TryGetInstance here because it won’t create concrete types
                return container.GetInstance(serviceType);
            } catch (StructureMapException) {
                return null;
            }
        }

        private object GetInterfaceService(Type serviceType) {
            return container.TryGetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}