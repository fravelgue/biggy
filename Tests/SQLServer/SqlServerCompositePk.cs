﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Biggy;
using Biggy.SQLServer;

namespace Tests.SQLServer {

 

  [Trait("SQL Server Store with Composite PKs", "")]
  public class SQLServerStoreWithCompositePk {
    IBiggyStore<Property> _propertyStore;
    IBiggyStore<Building> _buildingStore;
    public SQLServerStoreWithCompositePk() {
      var context = new SQLServerCache("chinook");

      // Build a table to play with from scratch each time:
      if (context.TableExists("Property")) {
        context.DropTable("Property");
      }

      if (context.TableExists("Building")) {
        context.DropTable("Building");
      }

      var columnDefs = new List<string>();
      columnDefs.Add("PropertyId int NOT NULL");
      columnDefs.Add("BuildingId int NOT NULL");
      columnDefs.Add("Name Text NOT NULL");
      columnDefs.Add("PRIMARY KEY (PropertyId, BuildingId)");
      context.CreateTable("Building", columnDefs);

      _propertyStore = new SQLServerStore<Property>(context);
      _buildingStore = new SQLServerStore<Building>(context);

    }

    [Fact(DisplayName = "Initializes store with Composite Keys")]
    public void Intialize_With_Injected_Context() {
      Assert.True(_buildingStore != null);
    }

    [Fact(DisplayName = "Inserts single record with composite key")]
    public void Inserts_Record_With_Composite_PK() {
      var newBuilding = new Building { PropertyId = 1, BuildingId = 1, Name = "Building A" };
      _buildingStore.Add(newBuilding);

      var fetchBuilding = _buildingStore.Load().First();
      Assert.True(fetchBuilding.PropertyId == 1 && fetchBuilding.BuildingId == 1 && fetchBuilding.Name == "Building A");
    }


    [Fact(DisplayName = "Inserts multiple records with composite keys")]
    public void Inserts_Multiple_Records_With_Composite_PK() {
      var list = new List<Building>();
      for (int i = 1; i <= 10; i++) {
        list.Add(new Building { PropertyId = 1, BuildingId = i, Name = "Building " + i });
      }
      _buildingStore.Add(list);

      var fetchBuildings = _buildingStore.Load();
      var checkBuilding = fetchBuildings.Last();
      Assert.True(fetchBuildings.Count() == 10 && checkBuilding.BuildingId == 10);
    }


    [Fact(DisplayName = "Updates single record with composite key")]
    public void Updates_Record_With_Composite_PK() {
      var list = new List<Building>();
      for (int i = 1; i <= 10; i++) {
        list.Add(new Building { PropertyId = 1, BuildingId = i, Name = "Building " + i });
      }
      _buildingStore.Add(list);

      var updateMe = _buildingStore.Load().FirstOrDefault(b => b.PropertyId == 1 && b.BuildingId == 3);
      updateMe.Name = "Updated Number 3";
      _buildingStore.Update(updateMe);

      var fetchBuilding = _buildingStore.Load().FirstOrDefault(b => b.PropertyId == 1 && b.BuildingId == 3);
      Assert.True(fetchBuilding.PropertyId == 1 && fetchBuilding.BuildingId == 3 && fetchBuilding.Name == "Updated Number 3");
    }

    [Fact(DisplayName = "Deletes single record with composite key")]
    public void Deletes_Single_Record_With_Composite_PK() {
      var list = new List<Building>();
      for (int i = 1; i <= 10; i++) {
        list.Add(new Building { PropertyId = 1, BuildingId = i, Name = "Building " + i });
      }
      int initialCount = list.Count;
      _buildingStore.Add(list);

      var deleteMe = _buildingStore.Load().FirstOrDefault(b => b.PropertyId == 1 && b.BuildingId == 3);
      _buildingStore.Remove(deleteMe);

      var fetchBuildings = _buildingStore.Load();
      Assert.True(initialCount == 10 && fetchBuildings.Count() == 9);
    }

    [Fact(DisplayName = "Deletes range of records with composite keys")]
    public void Deletes_Range_of_Records_With_Composite_PK() {
      var list = new List<Building>();
      for (int i = 1; i <= 10; i++) {
        list.Add(new Building { PropertyId = 1, BuildingId = i, Name = "Building " + i });
      }
      int initialCount = list.Count;
      _buildingStore.Add(list);

      var deleteMe = _buildingStore.Load().Where(b => b.PropertyId == 1 && b.BuildingId > 5);
      _buildingStore.Remove(deleteMe.ToList());

      var fetchBuildings = _buildingStore.Load();
      Assert.True(initialCount == 10 && fetchBuildings.Count() == 5);
    }
  }
}
