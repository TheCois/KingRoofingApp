// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using StructureMap;
using System.Configuration;
using System;
using ObjectFactory = KRF.Persistence.ObjectFactory;

namespace KRF.DependencyResolution {
    public static class IoC
    {

        //static string _connectionString = ConfigurationManager.ConnectionStrings["ConnectionName"].ConnectionString;
        static string _connectionString = Convert.ToString(ConfigurationManager.AppSettings["ApplicationDSN"]);

        public static IContainer Initialize() {
            return ObjectFactory.Container;
        }
    }
}