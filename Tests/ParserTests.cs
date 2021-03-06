﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sticky;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {


        [TestMethod]
        public void CanParseShakepeare()
        {

            var factSource = @"
CREATE (shakespeare:Author {firstname:'William', lastname:'Shakespeare'}),
 (juliusCaesar:Play {title:'Julius Caesar'}),
 (shakespeare)-[:WROTE_PLAY {year:1599}]->(juliusCaesar),
 (theTempest:Play {title:'The Tempest'}),
 (shakespeare)-[:WROTE_PLAY {year:1610}]->(theTempest),
 (rsc:Company {name:'RSC'}),
 (production1:Production {name:'Julius Caesar'}),
 (rsc)-[:PRODUCED]->(production1),
 (production1)-[:PRODUCTION_OF]->(juliusCaesar),
 (performance1:Performance {date:20120729}),
 (performance1)-[:PERFORMANCE_OF]->(production1),
 (production2:Production {name:'The Tempest'}),
 (rsc)-[:PRODUCED]->(production2),
 (production2)-[:PRODUCTION_OF]->(theTempest),
 (performance2:Performance {date:20061121}),
 (performance2)-[:PERFORMANCE_OF]->(production2),
 (performance3:Performance {date:20120730}),
 (performance3)-[:PERFORMANCE_OF]->(production1),
 (billy:User {name:'Billy'}),
 (review:Review {rating:5, review:'This was awesome!'}),
 (billy)-[:WROTE_REVIEW]->(review),
 (review)-[:RATED]->(performance1),
 (theatreRoyal:Venue {name:'Theatre Royal'}),
 (performance1)-[:VENUE]->(theatreRoyal),
 (performance2)-[:VENUE]->(theatreRoyal),
 (performance3)-[:VENUE]->(theatreRoyal),
 (greyStreet:Street {name:'Grey Street'}),
 (theatreRoyal)-[:STREET]->(greyStreet),
 (newcastle:City {name:'Newcastle'}),
 (greyStreet)-[:CITY]->(newcastle),
 (tyneAndWear:County {name:'Tyne and Wear'}),
 (newcastle)-[:COUNTY]->(tyneAndWear),
 (england:Country {name:'England'}),
 (tyneAndWear)-[:COUNTRY]->(england),
 (stratford:City {name:'Stratford upon Avon'}),
 (stratford)-[:COUNTRY]->(england),
 (rsc)-[:BASED_IN]->(stratford),
 (shakespeare)-[:BORN_IN]->(stratford);
";
            var host = new Host();
            host.Execute(factSource);

            var querySource = @"MATCH (theater:Venue {name:'Theatre Royal'}),
 (newcastle:City {name:'Newcastle'}),
 (bard:Author {lastname:'Shakespeare'}),
 (newcastle)<-[:STREET|CITY*1..2]-(theater)<-[:VENUE]-()-[:PERFORMANCE_OF]->()-[:PRODUCTION_OF]->(play)<-[:WROTE_PLAY]-(bard)
RETURN DISTINCT play.title AS play
";

            var result = host.Execute(querySource);
            Assert.AreEqual("Julius Caesar", result[0, "play"]);
            Assert.AreEqual("The Tempest", result[1, "play"]);
        }

    }
}
