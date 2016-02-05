using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sticky;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void CanParseMultipleCommands()
        {
            var source = @"
CREATE (email_1:Email {id:'1', content:'email contents'}),
 (bob)-[:SENT]->(email_1),
 (email_1)-[:TO]->(charlie),
 (email_1)-[:CC]->(davina),
 (email_1)-[:CC]->(alice),
 (email_1)-[:BCC]->(edward);
CREATE (email_2:Email {id:'2', content:'email contents'}),
 (bob)-[:SENT]->(email_2),
 (email_2)-[:TO]->(davina),
 (email_2)-[:BCC]->(edward);
CREATE (email_3:Email {id:'3', content:'email contents'}),
 (davina)-[:SENT]->(email_3),
 (email_3)-[:TO]->(bob),
 (email_3)-[:CC]->(edward);
CREATE (email_4:Email {id:'4', content:'email contents'}),
 (charlie)-[:SENT]->(email_4),
 (email_4)-[:TO]->(bob),
 (email_4)-[:TO]->(davina),
 (email_4)-[:TO]->(edward);
CREATE (email_5:Email {id:'5', content:'email contents'}),
 (davina)-[:SENT]->(email_5),
 (email_5)-[:TO]->(alice),
 (email_5)-[:BCC]->(bob),
 (email_5)-[:BCC]->(edward);
";
        }

        [TestMethod]
        public void CanParseShakepeare()
        {

            var source = @"
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
            host.Execute(source);
        }
    }
}
