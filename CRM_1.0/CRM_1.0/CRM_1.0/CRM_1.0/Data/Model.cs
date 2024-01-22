using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;

public class DB_Wittek : DbContext
{
    public DbSet<Person> Person { get; set; }
    public DbSet<Anfrage> Anfragen { get; set; }
    public DbSet<Angebot> Angebote { get; set; }
    public DbSet<Projekt> Projekte { get; set; }
    public DbSet<Service> Services { get; set; }


    public string DbPath { get; }

    public DB_Wittek()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "blogging.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql("Host=localhost; Database=postgres; Username=postgres; Password=postgres;");
}

public class Person
{
    public int PersonID { get; set; }
    public string Anrede1 { get; set; }
    public string Titel1 { get; set; }
    public string Nachname1 { get; set; }
    public string Vorname1 { get; set; }
    public string TelNr1 { get; set; }
    public string MobilNr1 { get; set; }
    public string Email1 { get; set; }
    public DateTime Geburtstag1 { get; set; }
    public string Sternzeichen1 { get; set; }
    public string Anrede2 { get; set; }
    public string Titel2 { get; set; }
    public string Nachname2 { get; set; }
    public string Vorname2 { get; set; }
    public string TelNr2 { get; set; }
    public string MobilNr2 { get; set; }
    public string Email2 { get; set; }
    public DateTime Geburtstag2 { get; set; }
    public string Sternzeichen2 { get; set; }
    public int PlzBst { get; set; }
    public string OrtBest { get; set; }
    public string StraßeBst { get; set; }
    public decimal NrBest { get; set; }
    public string Art { get; set; }
    public string Leistung { get; set; }
    public int PlzWhng { get; set; }
    public string OrtWhng { get; set; }
    public string StraßeWhng { get; set; }
    public string NrWhng { get; set; }
    public string Wiezuuns { get; set; }
    public string Sonstiges { get; set; }
    public DateTime Erstkontakt { get; set; }
    public DateTime Termin { get; set; }
    public bool AKTIV { get; set; }
}

public class Anfrage
{
    public int AnfrageID { get; set; }
    public DateTime AnfrageDatum { get; set; }
    public string Kostenintervall { get; set; }
    public string Zeitintervall { get; set; }
    public string Anfragestatus { get; set; }
    public int PersonID { get; set; }
    public Person Person { get; set; }
}

public class Angebot
{
    public int AngebotID { get; set; }
    public string Leistung { get; set; }
    public string Heizgaszulaenge { get; set; }
    public decimal Wirkungsgrad { get; set; }
    public string Heizintervall { get; set; }
    public decimal HolzMin { get; set; }
    public decimal HolzMax { get; set; }
    public string Kacheltyp { get; set; }
    public string Glasur1 { get; set; }
    public string Glasur2 { get; set; }
    public string Fugenmasse { get; set; }
    public string Heiztuer { get; set; }
    public string Steuerung { get; set; }
    public string Verkabelung { get; set; }
    public string Zuluft { get; set; }
    public string Notiz { get; set; }
    public bool Aktiv { get; set; }
    public string Angebotsstatus {  get; set; }
    public int AnfrageID { get; set; }
    public Anfrage Anfrage { get; set; }
}

public class Projekt
{
    public int ProjektID { get; set; }
    public DateTime DatumInbetriebnahme { get; set; }
    public DateTime DatumOfenzeremonie { get; set; }
    public int AngebotID { get; set; }
    public Angebot Angebot { get; set; }
}

public class Service
{
    public int ServiceID { get; set; }
    public string Art { get; set; }
    public DateTime Datum { get; set; }
    public string Information { get; set; }
    public decimal Kosten { get; set; }
    public bool Aktiv { get; set; }
    public int ProjektID { get; set; }
    public Projekt Projekt { get; set; }
}



