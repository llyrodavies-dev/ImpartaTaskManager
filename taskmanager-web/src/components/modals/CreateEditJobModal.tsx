interface CreateEditJobModal {
    editJob: { id: string; title: string; description: string } | null;
    setEditJob: (job: { id: string; title: string; description: string } | null) => void;
    onSave: (data: { id: string; title: string; description: string }) => void;
    editLoading: boolean;
}

export default function CreateEditJobModal({
    editJob,
    setEditJob,
    onSave,
    editLoading,
}: CreateEditJobModal) {
    if (!editJob) return null;

    return (
        <div className="fixed inset-0 flex items-center justify-center z-50"
            style={{ background: 'rgba(0,0,0,0.4)' }} >
            <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-md">
                <h3 className="text-xl font-bold mb-4 text-blue-800">
                    {editJob.id ? 'Edit Job' : 'Create Job'}
                </h3>
                <label className="block mb-2 text-sm font-medium text-gray-700">Title</label>
                <input
                    type="text"
                    className="w-full mb-4 px-3 py-2 border rounded"
                    value={editJob.title}
                    onChange={e => setEditJob({ ...editJob, title: e.target.value })}
                />
                <div className="flex justify-end gap-2">
                    <button
                        className="px-4 py-2 rounded bg-gray-300 hover:bg-gray-400 text-gray-800"
                        onClick={() => setEditJob(null)}
                        disabled={editLoading}
                    >
                        Cancel
                    </button>
                    <button
                        className="px-4 py-2 rounded bg-blue-600 hover:bg-blue-700 text-white"
                        onClick={() => onSave(editJob)}
                        disabled={editLoading}
                    >
                        {editLoading ? (editJob.id ? 'Saving...' : 'Creating...') : (editJob.id ? 'Save' : 'Create')}
                    </button>
                </div>
            </div>
        </div>
    );
}