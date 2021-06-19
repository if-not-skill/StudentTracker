using Microsoft.AspNetCore.Mvc;
using Xunit;
using StudentTracker.Controllers;
using StudentTracker.Models;
using System.Collections.Generic;
using System;

namespace StudentTracker.Tests
{
    public class StudentControllerTests
    {
        [Fact]
        public void GetLastStatesDateTest()
        {
            StudentsController controller = new StudentsController();

            ICollection<StudentState> studentStates = new List<StudentState>();
            studentStates.Add(new StudentState { StatusDate = new DateTime(2021, 6, 1) });
            studentStates.Add(new StudentState { StatusDate = new DateTime(2021, 6, 3) });

            Student student = new Student() { StudentStates = studentStates };

            var ReturnDate = controller.GetLastStatesDate(student);

            Assert.Equal(new DateTime(2021, 6, 3), ReturnDate);
        }

        [Fact]
        public void GetLastStatesDateZeroTest()
        {
            StudentsController controller = new StudentsController();

            ICollection<StudentState> studentStates = new List<StudentState>();

            Student student = new Student() { StudentStates = studentStates };

            var ReturnDate = controller.GetLastStatesDate(student);

            Assert.Equal(new DateTime(), ReturnDate);
        }

        [Fact]
        public void GetLastStatesDateTest2()
        {
            StudentsController controller = new StudentsController();

            ICollection<StudentState> studentStates = new List<StudentState>();
            studentStates.Add(new StudentState { StatusDate = new DateTime(2021, 6, 10) });
            studentStates.Add(new StudentState { StatusDate = new DateTime(2021, 5, 1) });
            studentStates.Add(new StudentState { StatusDate = new DateTime(2021, 6, 3) });

            Student student = new Student() { StudentStates = studentStates };

            var ReturnDate = controller.GetLastStatesDate(student);

            Assert.Equal(new DateTime(2021, 6, 10), ReturnDate);
        }

        [Fact]
        public void GetLastStatesDateTest3()
        {
            StudentsController controller = new StudentsController();

            ICollection<StudentState> studentStates = new List<StudentState>();
            studentStates.Add(new StudentState { StatusDate = new DateTime(2021, 5, 1) });

            Student student = new Student() { StudentStates = studentStates };

            var ReturnDate = controller.GetLastStatesDate(student);

            Assert.Equal(new DateTime(2021, 5, 1), ReturnDate);
        }

        [Fact]
        public void GetPathToFileTest()
        {
            StudentsController controller = new StudentsController();

            var ReturnPath = controller.GetPathToFile();

            Assert.Equal("\\templates\\EmailTemplate\\UpdateStudentStatus.html", ReturnPath);
        }
    }
}
