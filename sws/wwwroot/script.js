// Fetch and display documents

/* TODO, uncomment when fetch-documents object is available 
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
*/

//search 
document.getElementById('search-form').addEventListener('submit', async function (event) {
    event.preventDefault(); // Prevent default form submission

    const query = document.getElementById('search-query').value;
    const resultsContainer = document.getElementById('search-results');

    resultsContainer.innerHTML = '<p>Loading...</p>';

    try {
        // Make the API call to the backend
        const response = await fetch(`/api/documentsearch/search?query=${encodeURIComponent(query)}`);
        if (!response.ok) throw new Error('Failed to fetch search results');

        const results = await response.json();

        // Update the UI with results
        if (results.length === 0) {
            resultsContainer.innerHTML = '<p>No results found.</p>';
        } else {
            resultsContainer.innerHTML = results
                .map(result => `
                    <div style="margin-bottom: 1em; padding: 1em; background: #fff; border: 1px solid #ddd; border-radius: 8px;">
                        <strong>${result.name}</strong><br>
                        <small>${result.tags}</small><br>
                        <small>Created at: ${new Date(result.createdAt).toLocaleString()}</small>
                    </div>
                `)
                .join('');
        }
    } catch (error) {
        console.error('Error fetching search results:', error);
        resultsContainer.innerHTML = '<p style="color: red;">An error occurred while fetching search results.</p>';
    }



// Handle file upload
document.getElementById('add-document-form').addEventListener('submit', function (e) {
    e.preventDefault();


    const formData = new FormData();
    const name = document.getElementById('document-name').value;
    const file = document.getElementById('document-file').files[0];

    console.log("Uploading document " + name);

    formData.append('Name', name);
    formData.append('File', file);

    fetch('/api/UploadDocument', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                alert('Document added successfully!');
                document.getElementById('add-document-form').reset();
                //TODO: uncomment
                //document.getElementById('fetch-documents').click();  // Refresh document list
            } else {
                alert('Error adding document.');
            }
        })
        .catch(error => {
            alert('Error: Unable to add document.');
        });
});

/* TODO Uncomment, when uncommenting the top comment

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
*/