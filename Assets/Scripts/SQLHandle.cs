using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQLHandle : MonoBehaviour
{
    void Start()
    {
        TestInsert();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void TestInsert()
    {
        //string query = "CREATE TABLE user (pseudo VARCHAR(20))";
        //string connString = "Server=dpg-co2oqhol6cac73bs4i80-a;Database=valou_test_name;User Id=valou_test_name_user;Password=PTGBNqXukaTL8BQcz8MXAm3qHwKGv2QE;";
        

        
        
        //using (SqlConnection connection = new SqlConnection(connString))
        //{
        //    await connection.OpenAsync();
        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    {
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            // Lecture des données et affichage
        //            while (await reader.ReadAsync())
        //            {
        //                Console.WriteLine(reader);
        //                Console.WriteLine(reader.GetString(0));
        //            }
        //        }
        //    }
        //}
    }
}
