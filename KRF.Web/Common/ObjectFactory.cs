using KRF.Common;
using KRF.Core.FunctionalContracts;
using KRF.Core.Repository;
using KRF.Core.Repository.MISC;
using KRF.Persistence.RepositoryImplementation;
using KRF.Persistence.FunctionalContractImplementation;
using System.Configuration;
using StructureMap;
using System;
using System.Threading;
using KRF.Mail;
using KRF.Persistence;

namespace KRF.Web
{
    public static class ObjectFactory
    {
        //static string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionName"].ConnectionString;
        private static string _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);

        private static readonly Lazy<Container> _containerBuilder =
                new Lazy<Container>(DefaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Container
        {
            get { return _containerBuilder.Value; }
        }

        private static Container DefaultContainer()
        {
            return new Container(x =>
            {
                x.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    //scan.Assembly("KRF.Persistence");
                    scan.WithDefaultConventions();


                });

                x.For<IDatabaseConnection>().Use<DatabaseConnection>().Ctor<string>().Is(_connectionString);

                x.For<ILoginRepository>().Use<LoginRepository>();
                x.For<ILogin>().Use<Login>();

                x.For<IItemManagement>().Use<ItemManagement>();
                x.For<IItemManagementRepository>().Use<ItemManagementRepository>();

                x.For<IAssemblyManagement>().Use<AssemblyManagement>();
                x.For<IAssemblyManagementRepository>().Use<AssemblyManagementRepository>();

                x.For<IProspectManagement>().Use<ProspectManagement>();
                x.For<IProspectManagementRepository>().Use<ProspectManagementRepository>();

                x.For<ILeadManagement>().Use<LeadManagement>();
                x.For<ILeadManagementRepository>().Use<LeadManagementRepository>();

                //x.ForConcreteType<DatabaseConnection>().Configure.SetProperty(c => c.ConnectionString = _connectionString);

                x.For<IEstimateManagement>().Use<EstimateManagement>();
                x.For<IEstimateManagementRepository>().Use<EstimateManagementRepository>();
                x.For<IEmployeeManagementRepository>().Use<EmployeeManagementRepository>();
                x.For<IEmployeeManagement>().Use<EmployeeManagement>();
                x.For<ICrewManagementRepository>().Use<CrewManagementRepository>();
                x.For<ICrewManagement>().Use<CrewManagement>();
                x.For<IVendorManagementRepository>().Use<VendorManagementRepository>();
                x.For<IVendorManagement>().Use<VendorManagement>();
                x.For<IEquipmentManagementRepository>().Use<EquipmentManagementRepository>();
                x.For<IEquipmentManagement>().Use<EquipmentManagement>();
                x.For<IFleetManagementRepository>().Use<FleetManagementRepository>();
                x.For<IFleetManagement>().Use<FleetManagement>();
                x.For<IJobManagementRepository>().Use<JobManagementRepository>();
                x.For<IJobManagement>().Use<JobManagement>();

                x.For<IAdministrationManagement>().Use<AdministrationManagement>();
                x.For<IAdministrationRepository>().Use<AdministrationRepository>();

                x.For<IRolePermissionManagement>().Use<RolePermissionManagement>();
                x.For<IRolePermissionRepository>().Use<RolePermissionRepository>();
                x.For<IMailService>().Use<MailServiceImpl>();

                x.For<IZipCode>().Use<ZipCode>();
                x.For<IZipCodeRepository>().Use<ZipCodeRepository>();
            });
        }

        public static T GetInstance<T>()
        {
            return Container.GetInstance<T>();
        }
    }
}