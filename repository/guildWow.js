module.exports = guildWow;

function guildWow(connection) {
  this.db = connection;

  this.setup = function () {
    const sql = this.db.prepare(
      "CREATE TABLE IF NOT EXISTS guildWow ('guildID' VARCHAR PRIMARY KEY, 'region' VARCHAR, 'realm' VARCHAR, 'guild' VARCHAR)"
    );
    sql.run();
    console.log("\tGuild-Wow: ✓");
  };

  this.action = function (guildId, region, realm, guildName) {
    const sql = this.db.prepare(
      "INSERT INTO guildWow (guildId, region, realm, guild) VALUES (?, ?, ?, ?)"
    );
    sql.run(guildId, region, realm, guildName);
  };
}
