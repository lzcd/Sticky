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
CREATE (email_6:Email {id:'6', content:'email'}),
 (bob)-[:SENT]->(email_6),
 (email_6)-[:TO]->(charlie),
 (email_6)-[:TO]->(davina);
CREATE (reply_1:Email:Reply {id:'7', content:'response'}),
 (reply_1)-[:REPLY_TO]->(email_6),
 (davina)-[:SENT]->(reply_1),
 (reply_1)-[:TO]->(bob),
 (reply_1)-[:TO]->(charlie);
CREATE (reply_2:Email:Reply {id:'8', content:'response'}),
 (reply_2)-[:REPLY_TO]->(email_6),
 (bob)-[:SENT]->(reply_2),
 (reply_2)-[:TO]->(davina),
 (reply_2)-[:TO]->(charlie),
 (reply_2)-[:CC]->(alice);
CREATE (reply_3:Email:Reply {id:'9', content:'response'}),
 (reply_3)-[:REPLY_TO]->(reply_1),
 (charlie)-[:SENT]->(reply_3),
 (reply_3)-[:TO]->(bob),
 (reply_3)-[:TO]->(davina);
CREATE (reply_4:Email:Reply {id:'10', content:'response'}),
 (reply_4)-[:REPLY_TO]->(reply_3),
 (bob)-[:SENT]->(reply_4),
 (reply_4)-[:TO]->(charlie),
 (reply_4)-[:TO]->(davina);
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
