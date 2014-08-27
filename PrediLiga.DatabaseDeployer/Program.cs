﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AcklenAvenue.Data.NHibernate;
using DomainDrivenDatabaseDeployer;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using PrediLiga.Data;

namespace PrediLiga.DatabaseDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string connectionString = ConnectionStrings.Get();

            

            MsSqlConfiguration databaseConfiguration =
                MsSqlConfiguration.MsSql2008.ShowSql().ConnectionString(x => x.Is(connectionString));

            DomainDrivenDatabaseDeployer.DatabaseDeployer dd = null;
            ISessionFactory sessionFactory = new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                .Build(cfg => { dd = new DomainDrivenDatabaseDeployer.DatabaseDeployer(cfg); });

            using (ISession sess = sessionFactory.OpenSession())
            {
                using (IDbCommand cmd = sess.Connection.CreateCommand())
                {
                    //cmd.ExecuteSqlFile("dropForeignKeys.sql");
                    //cmd.ExecuteSqlFile("dropPrimaryKeys.sql");
                    //cmd.ExecuteSqlFile("dropTables.sql");
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Database dropped.");
            dd.Drop();
            Thread.Sleep(1000);

            dd.Create();
            Console.WriteLine("");
            Console.WriteLine("Database created.");

            ISession session = sessionFactory.OpenSession();
            using (ITransaction tx = session.BeginTransaction())
            {
                dd.Seed(new List<IDataSeeder>
                            {
                                new UserSeeder(session)
                            });
                tx.Commit();
            }
            session.Close();
            sessionFactory.Close();
            Console.WriteLine("");
            Console.WriteLine("Seed data added.");
            Thread.Sleep(2000);
        }
    }
}
