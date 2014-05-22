using System;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace JHGame.GameData
{
    public class Location
    {
        public string name{get;set;}
        public string description{get;set;}
        //public int x;
        //public int y;
    }

    public class MapRole
    {
        public string roleKey { get; set; }
        public string pic { get; set; }
        public string description { get; set; }
        public bool hide = true;
    }
    public enum EventRepeatType
    {
        Once,
        Unlimited,
    }
    public class Event
    {
        public string image { get; set; }
        public int lv = 0;
        public string description { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public EventRepeatType RepeatType { get; set; } 
        public int probability { get; set; }
        public List<EventCondition> conditions = new List<EventCondition>();

        static public Event Parse(XElement node)
        {
            Event aEvent = new Event();
            //推荐进入等级
            if (node.Attribute("lv") != null)
            {
                aEvent.lv = Tools.GetXmlAttributeInt(node, "lv");
            }
            if (node.Attribute("image") != null)
            {
                aEvent.image = Tools.GetXmlAttribute(node, "image");
            }
            else
            {
                aEvent.image = "";
            }
            if (node.Attribute("description") != null)
            {
                aEvent.description = Tools.GetXmlAttribute(node, "description");
            }
            
            aEvent.Value = Tools.GetXmlAttribute(node, "value");
            aEvent.Type = Tools.GetXmlAttribute(node, "type");
            if (node.Attribute("probability") == null)
            {
                aEvent.probability = 100;
            }
            else
            {
                aEvent.probability = Tools.GetXmlAttributeInt(node, "probability");
            }

            if (node.Attribute("repeat") != null && node.Attribute("repeat").Value == "once")
            {
                aEvent.RepeatType = EventRepeatType.Once;
            }
            else
            {
                aEvent.RepeatType = EventRepeatType.Unlimited;
            }
            aEvent.conditions = new List<EventCondition>();
            foreach (XElement eventCondition in node.Elements("condition"))
            {
                EventCondition eventCont = new EventCondition();
                eventCont.type = Tools.GetXmlAttribute(eventCondition, "type");
                eventCont.value = Tools.GetXmlAttribute(eventCondition, "value");
                if (eventCondition.Attribute("number") != null)
                    eventCont.number = Tools.GetXmlAttributeInt(eventCondition, "number");
                else
                    eventCont.number = 1;
                aEvent.conditions.Add(eventCont);
            }
            return aEvent;
        }
    };

    public class BigMap
    {
        public string Name;
        public string desc = "";
        public ImageSource Background;
        public List<Location> locationList = new List<Location>();
        public List<MapRole> roleList = new List<MapRole>();
        public Dictionary<string, List<Event>> eventsList = new Dictionary<string, List<Event>>();
        public List<string> Musics = new List<string>();

        public List<Event> getEvents(string locationName)
        {
            if (eventsList.ContainsKey(locationName))
            {
                return eventsList[locationName];
            }

            return null;
        }

        public List<Location> getLocations()
        {
            return locationList;
        }

        public List<MapRole> getMapRoles()
        {
            return roleList;
        }

        public string GetRandomMusic()
        {
            int k = (int)Tools.GetRandom(0, Musics.Count);
            if (k >= Musics.Count)
                k = 0;
            return Musics[k];
        }
    }

    public class EventCondition
    {
        public string type  { get; set; }
        public string value { get; set; }
        public int number = 1;

        public static EventCondition Parse(XElement node)
        {
            EventCondition rst = new EventCondition();
            rst.type = Tools.GetXmlAttribute(node, "type");
            rst.value = Tools.GetXmlAttribute(node, "value");
            if (node.Attribute("number") != null)
            {
                rst.number = Tools.GetXmlAttributeInt(node, "number");
            }
            return rst;
        }
    };

    public class MapEventsManager
    {
        static private List<BigMap> bigMaps = new List<BigMap>();

        static public void Init()
        {
            foreach (var eventXmlFile in ProjectFiles.GetFiles("map"))
            {
                XElement xmlRoot = Tools.LoadXml(Application.dataPath + eventXmlFile);
                XElement mapsRoot = Tools.GetXmlElement(xmlRoot, "maps");

                foreach (var map in Tools.GetXmlElements(mapsRoot, "map"))
                {
                    BigMap bigMap = new BigMap();
                    bigMap.Name = Tools.GetXmlAttribute(map, "name");
                    if (map.Attribute("desc") != null)
                        bigMap.desc = Tools.GetXmlAttribute(map, "desc");

					bigMap.Background = Tools.GetImage(ResourceManager.Get(Tools.GetXmlAttribute(map, "pic")));
                    if (map.Element("musics") != null)
                    {
                        foreach (var music in map.Element("musics").Elements("music"))
                        {
                            bigMap.Musics.Add(Tools.GetXmlAttribute(music, "name"));
                        }
                    }
                    foreach (XElement t in map.Elements("maprole"))
                    {
                        string roleKey = Tools.GetXmlAttribute(t, "roleKey");
                        MapRole mapRole = new MapRole();
                        mapRole.roleKey = roleKey;
                        if (t.Attribute("pic") != null)
                        {
                            mapRole.pic = Tools.GetXmlAttribute(t, "pic");
                        }
                        else
                        {
                            mapRole.pic = null;
                        }
                        if (t.Attribute("description") != null)
                        {
                            mapRole.description = Tools.GetXmlAttribute(t, "description");
                        }
                        if (t.Attribute("hide") != null)
                        {
                            if (Tools.GetXmlAttributeInt(t, "hide") == 0)
                            {
                                mapRole.hide = false;
                            }
                        }
                        bigMap.roleList.Add(mapRole);

                        List<Event> events = new List<Event>();
                        events.Clear();
                        foreach (XElement eventX in t.Elements("event"))
                        {
                            
                            events.Add(Event.Parse(eventX));
                        }

                        bigMap.eventsList.Add(roleKey, events);
                    }
                    bigMaps.Add(bigMap);
                }
            }
        }

        static public BigMap GetBigMap(string name)
        {
            foreach (var t in bigMaps)
            {
                if (t.Name.Equals(name)) return t;
            }
            return null;
        }
    }
}
