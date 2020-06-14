const Promise = require("bluebird");
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
  };

  this.setupDatabase = function (token) {
    return Promise.resolve(console.log("Ensuring database correctly setup:"))
      .then(this.blizzard.setup(token))
      .then(this.guildWow.setup())
      .then(console.log());
  };

  this.close = function () {
    console.log("Closing Database connection...");
    this.sql.close();
    console.log("Database connection closed");
  };

  this.constructor();
}
