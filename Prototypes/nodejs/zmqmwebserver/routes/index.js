const express = require("express");
const router = express.Router();
router.get("/", (req, res) => {
  res.send({ response: "This is web socket app, running.. so connect via socket client" }).status(200);
});
module.exports = router;