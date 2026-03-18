import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";

function CreateChatroomDialog({ open, creating, onClose, onCreate, error }) {
  const [title, setTitle] = useState("");

  useEffect(() => {
    if (!open) {
      setTitle("");
    }
  }, [open]);

  const handleCreate = () => {
    if (!title.trim() || creating) {
      return;
    }

    onCreate(title.trim());
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Create Chatroom</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          margin="dense"
          label="Chatroom title"
          fullWidth
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          disabled={creating}
        />
        {error && (
          <Typography variant="body2" color="error" sx={{ mt: 1 }}>
            {error}
          </Typography>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={creating}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleCreate}
          disabled={!title.trim() || creating}
        >
          Create
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export default CreateChatroomDialog;
