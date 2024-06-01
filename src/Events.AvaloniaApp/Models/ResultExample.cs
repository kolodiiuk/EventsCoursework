using System;
using Events.Domain;

namespace DesigningTypes
{
    public class ResultExample {
        public Result ProcessStudent(Student student) {
            return
                student.Enroll()
                    .OnFailure(() => Console.WriteLine("We need to enroll the student manually"))
                    .OnSuccess(() => 
                        student.PayGrant()
                               .OnFailure(() => Console.WriteLine("We need to process payment manually"))
                    );

        }
    }

    public class Student {
        public Result Enroll() {
            return Result.Success();
        }

        public Result PayGrant() {
            return Result.Fail("Doesn't deserve :)");
        }
    }
}
