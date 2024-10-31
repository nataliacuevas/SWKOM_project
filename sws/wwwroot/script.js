// Fetch and display documents
document.getElementById('fetch-documents').addEventListener('click', function () {
    fetch('/api/UploadDocument')
        .then(response => response.json())
        .then(data => {
            const documentList = document.getElementById('document-list');
            documentList.innerHTML = '';  // Clear previous data

            data.forEach(doc => {
                const docItem = document.createElement('div');
                docItem.classList.add('document');
                docItem.innerHTML = `
                    <strong>ID:</strong> ${doc.id} <br>
                    <strong>Name:</strong> ${doc.name} <br>
                    <button onclick="deleteDocument(${doc.id})" class="danger-btn">Delete</button>
                    <button onclick="updateDocument(${doc.id})" class="secondary-btn">Update</button>
                `;
                documentList.appendChild(docItem);
            });
        })
        .catch(error => {
            document.getElementById('document-list').textContent = 'Error: Unable to fetch documents';
        });
});

// Handle file upload
document.getElementById('add-document-form').addEventListener('submit', function (e) {
    e.preventDefault();

    const formData = new FormData();
    const name = document.getElementById('document-name').value;
    const file = document.getElementById('document-file').files[0];

    formData.append('name', name);
    formData.append('file', file);

    fetch('/api/UploadDocument', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                alert('Document added successfully!');
                document.getElementById('add-document-form').reset();
                document.getElementById('fetch-documents').click();  // Refresh document list
            } else {
                alert('Error adding document.');
            }
        })
        .catch(error => {
            alert('Error: Unable to add document.');
        });
});

// Delete a document
function deleteDocument(id) {
    if (confirm("Are you sure you want to delete this document?")) {
        fetch(`/api/UploadDocument/${id}`, { method: 'DELETE' })
            .then(response => {
                if (response.ok) {
                    alert('Document deleted successfully!');
                    document.getElementById('fetch-documents').click();  // Refresh document list
                } else {
                    alert('Error deleting document.');
                }
            })
            .catch(error => {
                alert('Error: Unable to delete document.');
            });
    }
}

// Update a document (simplified)
function updateDocument(id) {
    const name = prompt('Enter new name:');
    const content = prompt('Enter new content:');

    if (name && content) {
        fetch(`/api/UploadDocument/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, name, content })
        })
            .then(response => {
                if (response.ok) {
                    alert('Document updated successfully!');
                    document.getElementById('fetch-documents').click();  // Refresh document list
                } else {
                    alert('Error updating document.');
                }
            })
            .catch(error => {
                alert('Error: Unable to update document.');
            });
    }
}
