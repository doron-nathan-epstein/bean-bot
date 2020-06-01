const SQLite = require("better-sqlite3");
const blizzard = require("./blizzard.js");
const guildWow = require("./guildWow.js");

module.exports = context;

function context() {
  this.constructor = function () {
    console.log("Opening Database connection...");
    this.sql = new SQLite(process.env.DB_CONNECTION);
    this.blizzard = new blizzard(this.sql);
    this.guildWow = new guildWow(this.sql);
    console.log("Database connection opened");
    this.setupDatabase();
    console.log("");
  };

  this.setupDatabase = function () {
    console.log("Ensuring database correctly setup:");
    this.blizzard.setup();
    this.guildWow.setup();
  };

  this.close = function () {
    console.log("Closing Database connection...");
    this.sql.close();
    console.log("Database connection closed");
  };

  this.constructor();
}
