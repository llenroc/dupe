using Abp;

namespace project56
{
    /// <summary>
    /// This class can be used as a base class for services in this application.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// It's suitable for non domain nor application service classes.
    /// For domain services inherit <see cref="project56DomainServiceBase"/>.
    /// For application services inherit project56AppServiceBase.
    /// </summary>
    public abstract class project56ServiceBase : AbpServiceBase
    {
        protected project56ServiceBase()
        {
            LocalizationSourceName = project56Consts.LocalizationSourceName;
        }
    }
}