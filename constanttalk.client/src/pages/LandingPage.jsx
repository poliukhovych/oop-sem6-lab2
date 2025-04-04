import AuthButton from "../components/AuthButton";
import { Container, Row, Col } from "react-bootstrap";

function LandingPage() {
  return (
    <div
      className="min-vh-100 d-flex align-items-center"
      style={{
        background: "linear-gradient(135deg, #2c3e50, #34495e)",
        color: "#ecf0f1",
      }}
    >
      <Container fluid className="text-center">
        <Row className="justify-content-center">
          <Col
            md={8}
            lg={6}
            className="p-5 rounded shadow-lg"
            style={{
              background: "rgba(52, 73, 94, 0.7)",
              backdropFilter: "blur(10px)",
              border: "1px solid rgba(255, 255, 255, 0.1)",
            }}
          >
            <h1
              className="display-3 fw-bold mb-4"
              style={{
                textShadow: "2px 2px 4px rgba(0, 0, 0, 0.5)",
                fontFamily: "Montserrat, sans-serif",
              }}
            >
              ConstantTalk
            </h1>
            <p
              className="lead mb-4"
              style={{
                fontStyle: "italic",
                opacity: 0.8,
              }}
            >
              Your trusted digital telephone service. Connect, communicate, and
              stay in touch effortlessly.
            </p>
            <div className="d-flex justify-content-center">
              <AuthButton />
            </div>
          </Col>
        </Row>
      </Container>
    </div>
  );
}

export default LandingPage;