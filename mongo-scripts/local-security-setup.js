// mongo 127.0.0.1/admin ./mongo-scripts/local-security-setup.js
// https://docs.mongodb.com/manual/reference/built-in-roles/

// The user corp-hq will connect with and run DB operations as

db.createUser(
  {
    user: "mongoUser",
    pwd: "mongoPass",
    roles: [
      { role: "readWrite", db: "corp-hq" }
    ]
  }
);

// The user you will connect with and run DB operations as
db.createUser(
  {
    user: "mongoOwner",
    pwd: "mongoPass",
    roles: [
      { role: "dbOwner", db: "corp-hq" }
    ]
  }
);
