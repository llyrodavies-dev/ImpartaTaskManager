interface EditTaskModal {
    editTask: { id: string; title: string; description: string } | null;
    setEditTask: (task: { id: string; title: string; description: string } | null) => void;
    onSave: (data: { id: string; title: string; description: string }) => void;
    editLoading: boolean;
}

export default function EditTaskModal({
    editTask,
    setEditTask,
    onSave,
    editLoading,
}: EditTaskModal) {
    if (!editTask) return null;

    return (
        <div className="fixed inset-0 flex items-center justify-center z-50"
            style={{ background: 'rgba(0,0,0,0.4)' }} >
            <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-md">
                <h3 className="text-xl font-bold mb-4 text-blue-800">Edit Task</h3>
                <label className="block mb-2 text-sm font-medium text-gray-700">Title</label>
                <input
                    type="text"
                    className="w-full mb-4 px-3 py-2 border rounded"
                    value={editTask.title}
                    onChange={e => setEditTask({ ...editTask, title: e.target.value })}
                />
                <label className="block mb-2 text-sm font-medium text-gray-700">Description</label>
                <textarea
                    className="w-full mb-4 px-3 py-2 border rounded"
                    value={editTask.description}
                    onChange={e => setEditTask({ ...editTask, description: e.target.value })}
                />
                <div className="flex justify-end gap-2">
                    <button
                        className="px-4 py-2 rounded bg-gray-300 hover:bg-gray-400 text-gray-800"
                        onClick={() => setEditTask(null)}
                        disabled={editLoading}
                    >
                        Cancel
                    </button>
                    <button
                        className="px-4 py-2 rounded bg-blue-600 hover:bg-blue-700 text-white"
                        onClick={() => onSave(editTask)}
                        disabled={editLoading}
                    >
                        {editLoading ? 'Saving...' : 'Save'}
                    </button>
                </div>
            </div>
        </div>
    );
}