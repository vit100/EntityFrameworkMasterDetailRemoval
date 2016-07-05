using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EWF
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var myContext = new MyContext())
            {
                var vit1 = myContext.People.Single(p => p.Name.Contains("vit"));
                myContext.Entry(vit1).Collection(p=>p.Cars).Load();

                //var cars = vit1.Cars.ToList();
                //foreach (var car in cars)
                //{
                //    myContext.Entry(car).State = EntityState.Deleted;    
                //}
                //myContext.Entry(vit1).State = EntityState.Deleted;


                myContext.Set(vit1.GetType()).Remove(vit1);

                myContext.SaveChanges();
            }
        }
    }

    public class MyContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public MyContext()
        {
            Database.SetInitializer(new MyInitializer());
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectContext_ObjectMaterialized;
        }

        private void ObjectContext_ObjectMaterialized(object sender, System.Data.Entity.Core.Objects.ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

//            modelBuilder.Entity<Car>().HasRequired(c=>)
        }


    }

    public class MyInitializer : DropCreateDatabaseAlways<MyContext>
    {
        protected override void Seed(MyContext context)
        {
            base.Seed(context);
            var vit = new Person()
            {
                Name = "vit",
                Cars = new List<Car>
                {
                    new Car() {Model="Aveo"},
                    new Car() {Model="Nissan"}
                }
            };
            context.People.Add(vit);
            context.SaveChanges();
        }
    }
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual List<Car> Cars { get; set; }
}

public class Car
{
    public int Id { get; set; }
    public string Model { get; set; }

    
    public Person Person { get; set; }
    [ForeignKey("Person")]
    public int PersonKey { get; set; }
}

