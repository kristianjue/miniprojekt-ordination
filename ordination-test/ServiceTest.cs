namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;
using static shared.Util;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestAtKodenSmiderEnException()
    {
            service.OpretDagligFast(0, 0, 0, 0, 0, 0, DateTime.Now, DateTime.Now.AddDays(3));

    }
    [TestMethod]
    public void OpretDagligSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligSkæve().Count());

        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligSkæve().Count());
    }

    //TEST AF DAGLIG SKÆV//


    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(10, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævLægemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(patient.PatientId, 10, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævPatientIdNegativ()
    {
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(-1, 1,doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævLægemiddelIdNegativ()
    {
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(1, -1, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da lægemiddel id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævDoserTom()
    {
        Dosis[] doser = new Dosis[] {};
        service.OpretDagligSkaev(1, 1, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da doser er tom");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkaevstartDatostørreendslutDato()
    {
        Dosis[] doser = new Dosis[] { new Dosis(CreateTimeOnly(12, 0, 0), 2) };
        service.OpretDagligSkaev(1, -1, doser, DateTime.Now.AddDays(3), DateTime.Now);
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, startDato kan ikke være større end slutDato");
    }


        //TEST AF PN//

        [TestMethod]
    public void OpretPN()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(4, service.GetPNs().Count());

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(5, service.GetPNs().Count());
    }


    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretPN(50, lm.LaegemiddelId,2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNLægemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretPN(patient.PatientId, 50, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNPatientIdNegativ()
    {
        service.OpretPN(-1, 1, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNLægemiddelIdNegativ()
    {
        service.OpretPN(1, -1, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da lægemiddel id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNAntalNegativ()
    {
        service.OpretPN(1, 1, -1, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da antal er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNstartDatostørreendslutDato()
    {
        service.OpretPN(1, 1, 1, DateTime.Now.AddDays(3), DateTime.Now);
        
        Console.WriteLine("oprettelse af PN fejlet korrekt, startDato kan ikke være større end slutDato");
    }



    //DAGLIG FAST TESTMETODER//


    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(10, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastLægemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, 8, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastPatientIdNegativ()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(-1,lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastLægemiddelIdNegativ()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, -1, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da lægemiddel id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFaststartDatostørreendslutDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, -1, 2, 2, 1, 0, DateTime.Now.AddDays(3), DateTime.Now);
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, startDato kan ikke være større end slutDato");
    }




    
}
