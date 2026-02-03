interface DeleteModalProps {
  open: boolean;
  itemName?: string;
  onConfirm: () => void;
  onCancel: () => void;
  loading?: boolean;
}

export default function DeleteModal({
  open,
  itemName,
  onConfirm,
  onCancel,
  loading,
}: DeleteModalProps) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 flex items-center justify-center z-50" style={{ background: 'rgba(0,0,0,0.4)' }}>
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-sm">
        <h3 className="text-xl font-bold mb-4 text-red-700">Confirm Deletion</h3>
        <p className="mb-6 text-gray-700">
          Are you sure you want to delete {itemName ? <b>{itemName}</b> : "this item"}? This action cannot be undone.
        </p>
        <div className="flex justify-end gap-2">
          <button
            className="px-4 py-2 rounded bg-gray-300 hover:bg-gray-400 text-gray-800"
            onClick={onCancel}
            disabled={loading}
          >
            Cancel
          </button>
          <button
            className="px-4 py-2 rounded bg-red-600 hover:bg-red-700 text-white"
            onClick={onConfirm}
            disabled={loading}
          >
            {loading ? "Deleting..." : "Delete"}
          </button>
        </div>
      </div>
    </div>
  );
}