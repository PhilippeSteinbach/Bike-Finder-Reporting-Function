using System;
using System.Collections.Generic;
using System.Text;

namespace Reporting
{
    public class History
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string bike { get; set; }
        public float distance { get; set; }
        public int count { get; set; }
        public string image { get; set; }

    }

    // Erstellt via JSON in Zwischenablage -> Bearbeiten -> Inhalte einfügen-> JSON als Klassen einfügen

    public class Rootobject
    {
        public int total { get; set; }
        public Item[] items { get; set; }
        public Status[] statuses { get; set; }
        public _Links _links { get; set; }
    }

    public class _Links
    {
        public Self self { get; set; }
        public Create create { get; set; }
        public CreatePublish createpublish { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Create
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class CreatePublish
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string createdBy { get; set; }
        public string lastModifiedBy { get; set; }
        public Data data { get; set; }
        public DateTime created { get; set; }
        public DateTime lastModified { get; set; }
        public string status { get; set; }
        public string statusColor { get; set; }
        public string schemaName { get; set; }
        public string schemaDisplayName { get; set; }
        public int version { get; set; }
        public _Links1 _links { get; set; }
    }

    public class Data
    {
        public Startdate startDate { get; set; }
        public Enddate endDate { get; set; }
        public Bike bike { get; set; }
        public Distance distance { get; set; }
        public Image image { get; set; }
    }

    public class Startdate
    {
        public DateTime iv { get; set; }
    }

    public class Enddate
    {
        public DateTime iv { get; set; }
    }

    public class Bike
    {
        public string iv { get; set; }
    }

    public class Distance
    {
        public float iv { get; set; }
    }

    public class Image
    {
        public string[] iv { get; set; }
    }

    public class _Links1
    {
        public Self1 self { get; set; }
        public Previous previous { get; set; }
        public DraftCreate draftcreate { get; set; }
        public Delete delete { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Previous
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class DraftCreate
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Delete
    {
        public string href { get; set; }
        public string method { get; set; }
    }

    public class Status
    {
        public string status { get; set; }
        public string color { get; set; }
    }
}
