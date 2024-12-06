
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# Install libs for Tesseract
#RUN apt-get update
#RUN apt-get install -y git cmake build-essential
#RUN mkdir leptonica
#RUN git clone https://github.com/DanBloomberg/leptonica.git /leptonica

#WORKDIR /leptonica
#RUN mkdir build
#WORKDIR /leptonica/build
#RUN cmake ..

RUN apt-get install -y libleptonica-dev libtesseract-dev ghostscript

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the csproj file and restore any dependencies (via npm)
COPY *.csproj ./
RUN dotnet restore


# Copy the rest of the application code
COPY . ./
RUN dotnet publish  -c Release -o out
# Use the official ASP.NET runtime image for the runtime environment
FROM base AS runtime
# Link libs for Tesseract
WORKDIR /app/x64
RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4.0.1 libtesseract50.so

WORKDIR /app
COPY --from=build /src/out .

# Copy any additional files like PDFs and trained data
COPY docs/ /app/docs/
COPY tessdata/ /app/tessdata/

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "OCRworker.dll"]
