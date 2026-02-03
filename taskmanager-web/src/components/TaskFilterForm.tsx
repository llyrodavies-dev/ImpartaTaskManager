import { useState } from 'react';
import type { FilterRequest } from '../models/FilterRequest';
import { TaskItemStatusLabels } from '../models/enums/TaskItemStatusLabels';

type FieldOption = { value: string; label: string };
type OperatorOption = { value: number; label: string };

interface TaskFilterFormProps {
    fieldOptions: FieldOption[];
    fieldOperators: { [fieldName: string]: OperatorOption[] };
    filters: FilterRequest[];
    onAddFilter: (filter: FilterRequest) => void;
    onDeleteFilter: (index: number) => void;
}

export default function TaskFilterForm({ fieldOptions, fieldOperators, filters, onAddFilter, onDeleteFilter }: TaskFilterFormProps) {
    const [filter, setFilter] = useState<FilterRequest>({
        FieldName: fieldOptions[0]?.value || '',
        Operator: fieldOperators[fieldOptions[0]?.value]?.[0]?.value ?? 1,
        Values: [''],
    });

    const operatorOptions = fieldOperators[filter.FieldName] || [];

    const handleAdd = () => {
        if (!filter.FieldName || !filter.Values[0]) return;
        onAddFilter({ ...filter, Values: [filter.Values[0]] });
        setFilter({
            FieldName: fieldOptions[0]?.value || '',
            Operator: fieldOperators[fieldOptions[0]?.value]?.[0]?.value ?? 1,
            Values: [''],
        });
    };

    const getFieldLabel = (value: string) =>
        fieldOptions.find(f => f.value === value)?.label || value;
    const getOperatorLabel = (value: number) =>
        fieldOperators[filter.FieldName]?.find(o => o.value === value)?.label || value;


    return (
        <div className="mb-6">
        <div className="flex items-end gap-4">
            <div>
                <label className="block text-sm font-medium">Field</label>
                <select
                    value={filter.FieldName}
                    onChange={e => setFilter(f => ({ ...f, FieldName: e.target.value, Operator: fieldOperators[e.target.value]?.[0]?.value ?? 1 }))}
                    className="border rounded px-2 py-1"
                >
                    {fieldOptions.map(opt => (
                        <option key={opt.value} value={opt.value}>{opt.label}</option>
                    ))}
                </select>
            </div>
            <div>
                <label className="block text-sm font-medium">Operator</label>
                <select
                    value={filter.Operator}
                    onChange={e => setFilter(f => ({ ...f, Operator: Number(e.target.value) }))}
                    className="border rounded px-2 py-1"
                >
                    {operatorOptions.map(opt => (
                        <option key={opt.value} value={opt.value}>{opt.label}</option>
                    ))}
                </select>
            </div>
            <div>
                <label className="block text-sm font-medium">Value</label>
                {filter.FieldName === 'Status' ? (
                    <select
                        value={filter.Values[0]}
                        onChange={e => setFilter(f => ({ ...f, Values: [e.target.value] }))}
                        className="border rounded px-2 py-1"
                    >
                        <option value="">Select status</option>
                        {Object.entries(TaskItemStatusLabels).map(([value, label]) => (
                            <option key={value} value={value}>{label}</option>
                        ))}
                    </select>
                ) : (
                    <input
                        type="text"
                        value={filter.Values[0]}
                        onChange={e => setFilter(f => ({ ...f, Values: [e.target.value] }))}
                        className="border rounded px-2 py-1"
                    />
                )}
            </div>
            <button
                onClick={handleAdd}
                className="bg-blue-600 text-white px-4 py-2 rounded"
            >
                Add Filter
            </button>
        </div>
        {filters.length > 0 && (
                <div className="mt-4">
                    <ul>
                        {filters.map((f, idx) => (
                            <li key={idx} className="flex items-center gap-2 mb-1">
                                <span className="px-2 py-1 bg-gray-100 rounded">
                                    {getFieldLabel(f.FieldName)} {getOperatorLabel(f.Operator)} "{f.Values[0]}"
                                </span>
                                <button
                                    onClick={() => onDeleteFilter(idx)}
                                    className="ml-2 text-red-600 hover:underline"
                                    title="Delete"
                                >
                                    Delete
                                </button>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
}