module.exports = {
  name: "setup-wow",
  description:
    "Setup the server to use a default World of Warcraft guild for WoW queries.",
  guildOnly: true,
  adminOnly: true,
  args: true,
  usage: "[region] [realm] [guild-name]",
  async execute(message, appDAO, args) {
    if (args.length !== 3) {
      return message.reply("Incorrect amount of arguments recieved.");
    }

    const region = args[0];
    if (!validateRegion(appDAO, region)) {
      return message.reply("region not valid.");
    }

    const realm = args[1];
    if (!validateRealm(appDAO, region, realm)) {
      return message.reply("realm not valid.");
    }

    const guildName = args[2];

    appDAO.guildwow.action(message.guild.id, region, realm, guildName);
  },
};

function validateRegion(appDAO, region) {
  return true;
}

function validateRealm(appDAO, region, realm) {
  return true;
}
