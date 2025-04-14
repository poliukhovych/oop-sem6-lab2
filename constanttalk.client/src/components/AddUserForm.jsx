import { useState } from "react";
import { Form, Button } from "react-bootstrap";

function AddUserForm({ onAddUser }) {
  const [name, setName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [email, setEmail] = useState("");
  const [auth0Id, setAuth0Id] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!auth0Id || !name || !phoneNumber || !email) return;
    onAddUser(auth0Id, name, phoneNumber, email);
    setAuth0Id("");
    setName("");
    setPhoneNumber("");
    setEmail("");
  };

  return (
    <Form onSubmit={handleSubmit} className="mb-4">
      <h3 className="mb-3">➕ Add Subscriber</h3>
      <Form.Group className="mb-2">
      <Form.Control
        type="text"
        placeholder="Auth0Id"
        value={auth0Id}
        onChange={(e) => setAuth0Id(e.target.value)}
      />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Control
          type="text"
          placeholder="Name"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Control
          type="text"
          placeholder="Phone number"
          value={phoneNumber}
          onChange={(e) => setPhoneNumber(e.target.value)}
        />
      </Form.Group>
      <Form.Group className="mb-2">
      <Form.Control
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      </Form.Group>
      <Button variant="primary" type="submit">
        Add
      </Button>
    </Form>
  );
}

export default AddUserForm;